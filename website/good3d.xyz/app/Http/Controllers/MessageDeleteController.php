<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use Illuminate\Support\Facades\DB;

class MessageDeleteController extends Controller
{
    public function delete(Request $request)
    {
        $ids = $request->input('messages', []);

        if (!is_array($ids)) {
            $ids = [$ids];
        }

        DB::table('messages')
            ->where('user_to', auth()->id())
            ->whereIn('id', $ids)
            ->delete();

        return redirect('/my/inbox');
    }
}