<?php

namespace App\Http\Controllers;
use App\Models\Game;
use App\Models\Favorite;
use Illuminate\Http\Request;
use Illuminate\Support\Facades\DB;

class GamesController extends Controller
{
    public function show($id)
    {
        $game = Game::with('creator')->findOrFail($id);
        $game->increment('visits');
        $favoritesCount = Favorite::where('itemid', $game->id)->count();
        $activeServers = DB::table('game_servers')
            ->where('game_id', $game->id)
            ->where('status', 1)
            ->get();

        return view('games.view_game', [
            'game' => $game,
            'favorites_count' => $favoritesCount,
            'active_servers' => $activeServers
        ]);
    }
}