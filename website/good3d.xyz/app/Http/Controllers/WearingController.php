<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use Illuminate\Support\Facades\DB;
use Illuminate\Support\Facades\Auth;

class WearingController extends Controller
{
    public function index()
    {
        $items = DB::table('wearing as w')
            ->join('catalog as c', 'w.itemid', '=', 'c.id')
            ->where('w.userid', Auth::id())
            ->select(
                'w.itemid',
                'c.id',
                'c.name',
                'c.type',
                'c.thumbnail',
                'c.filename',
                'c.creator_id',
                'c.creator_name'
            )
            ->get();

        return view('partials.wearing', compact('items'));
    }
}