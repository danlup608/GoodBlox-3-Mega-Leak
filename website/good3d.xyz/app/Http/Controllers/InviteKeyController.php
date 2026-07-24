<?php
namespace App\Http\Controllers;
use Illuminate\Http\Request;
use Illuminate\Support\Facades\DB;
use Illuminate\Support\Str;

class InviteKeyController extends Controller
{
    public function index()
    {
        $user = auth()->user();
        $myKeys = DB::table('invite_keys')
            ->where('made_by', $user->id)
            ->orderBy('id', 'desc')
            ->limit(10)
            ->get();
        return view('my.invite', compact('user', 'myKeys'));
    }

    public function generate(Request $request)
    {
        $user = auth()->user();
        if ($user->tix < 30) {
            return redirect()->back()->withErrors(['tix' => 'You do not have enough Tickets to purchase an invite key.']);
        }

        $newKey = 'GOODBLOX-' . strtoupper(Str::random(16));

        DB::transaction(function () use ($user, $newKey) {
            DB::table('users')
                ->where('id', $user->id)
                ->decrement('tix', 30);
                
            DB::table('users')
                ->where('id', 2)
                ->increment('tix', 30);
            DB::table('invite_keys')->insert([
                'invite_key' => $newKey,
                'used'       => 0,
                'made_by'    => $user->id, 
                'created_at' => now(),
            ]);
        });
        return redirect()->back()->with('success', 'Successfully generated invite key: ' . $newKey);
    }
}