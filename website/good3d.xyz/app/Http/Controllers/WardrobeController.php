<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use Illuminate\Support\Facades\DB;
use Illuminate\Support\Facades\Auth;

class WardrobeController extends Controller
{
    public function index(Request $request)
    {
        if (!Auth::check()) {
            return response('Not logged in', 401);
        }

        $type = $request->query('wtype', 'hat');
        $page = max(1, (int) $request->query('p', 1));

        $perPage = 8;
        $offset = ($page - 1) * $perPage;

        $totalItems = DB::table('owned_items as oi')
            ->join('catalog as c', 'oi.itemid', '=', 'c.id')
            ->where('oi.ownerid', Auth::id())
            ->where('c.type', $type)
            ->count();

        $totalPages = max(1, ceil($totalItems / $perPage));

        $items = DB::table('owned_items as oi')
            ->join('catalog as c', 'oi.itemid', '=', 'c.id')
            ->where('oi.ownerid', Auth::id())
            ->where('c.type', $type)
            ->orderByDesc('c.id')
            ->offset($offset)
            ->limit($perPage)
            ->select(
                'c.id',
                'c.name',
                'c.type',
                'c.thumbnail',
                'c.creator_id',
                'c.creator_name'
            )
            ->get();

        return view('partials.wardrobe', [
            'items' => $items,
            'type' => $type,
            'page' => $page,
            'totalPages' => $totalPages
        ]);
    }
}