<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use App\Models\User;
use App\Models\Message;
use App\Models\Game;

class UserController extends Controller
{
    public function index(Request $request)
    {
        $id = $request->query('ID');

        if (!$id) {

            if (auth()->check()) {

                return $this->myHome();

            }

            return redirect('/Browse.aspx');
        }

        $user = User::find($id);

        if (!$user) {
            abort(404);
        }

        return view('user.profile', compact('user'));
    }

    public function myHome()
    {
        $message = Message::where('readto', 0)
            ->where('user_to', auth()->id())
            ->count();

        $placesCount = Game::where('creatorid', auth()->id())
            ->count();

        $places = Game::where('creatorid', auth()->id())
            ->orderBy('id', 'desc')
            ->limit(10)
            ->get();

        $maxPlaces = 3;

        $remainingPlaces = max(0, $maxPlaces - $placesCount);

        return view('my.home', compact(
            'message',
            'remainingPlaces',
            'places'
        ));
    }
}