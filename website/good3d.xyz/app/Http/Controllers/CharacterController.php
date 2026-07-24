<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use Illuminate\Support\Facades\Auth;

class CharacterController extends Controller
{
    public function index()
    {
        $user = Auth::user();

        $robloxColors = [
            "#F2F3F2",
            "#E5E4DE",
            "#A3A2A4",
            "#635F61",
            "#1B2A34",
            "#C4281B",
            "#F5CD2F",
            "#FDEA8C",
            "#0D69AB",
            "#008F9B",
            "#6E99C9",
            "#80BBDB",
            "#B4D2E3",
            "#74869C",
            "#DA8540",
            "#E29B3F",
            "#27462C",
            "#287F46",
            "#4B974A",
            "#A4BD46",
            "#A1C48B",
            "#789081",
            "#A05F34",
            "#694027",
            "#6B327B",
            "#E8BAC7",
            "#DA8679",
            "#D7C599",
            "#957976",
            "#7C5C45",
            "#CC8E68",
            "#EAB891"
        ];

        return view('character', compact('user', 'robloxColors'));
    }

    public function changeBodyColor(Request $request)
    {
        $user = Auth::user();

        $request->validate([
            'part' => 'required',
            'color' => 'required'
        ]);

        $map = [
            'head' => 'headcolor',
            'torso' => 'torsocolor',
            'leftarm' => 'leftarmcolor',
            'rightarm' => 'rightarmcolor',
            'leftleg' => 'leftlegcolor',
            'rightleg' => 'rightlegcolor',
        ];

        if (!isset($map[$request->part])) {
            return response()->json([
                'success' => false
            ]);
        }

        $column = $map[$request->part];

        $user->$column = $request->color;
        $user->save();

        return response()->json([
            'success' => true
        ]);
    }

    public function changePose(Request $request)
    {
        $user = Auth::user();

        $user->pose = $request->pose;
        $user->save();

        return response()->json([
            'success' => true
        ]);
    }
}