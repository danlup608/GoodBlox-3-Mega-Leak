<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use Illuminate\Support\Facades\DB;

class ItemController extends Controller
{
    public function show(Request $request)
    {
        $id = (int)$request->query('ID');

        if (!$id) {
            abort(404);
        }

        $item = DB::table('catalog')
            ->where('id', $id)
            ->first();

        if (!$item) {
            abort(404);
        }

        $creator = DB::table('users')
            ->where('id', $item->creator_id)
            ->first();

        $comments = DB::table('comments')
            ->leftJoin('users', 'users.id', '=', 'comments.userid')
            ->where('comments.assetid', $id)
            ->select(
                'comments.*',
                'users.username',
                'users.user_role'
            )
            ->orderBy('comments.time_posted', 'desc')
            ->get();

        $favorites = DB::table('favorites')
            ->where('itemid', $id)
            ->count();

        $owned = false;

        if (auth()->check()) {

            $owned = DB::table('owned_items')
                ->where('itemid', $id)
                ->where('ownerid', auth()->id())
                ->exists();
        }

        $typeNames = [
            'hat' => 'Hat',
            'shirt' => 'Shirt',
            'pants' => 'Pants',
            'tshirt' => 'T-Shirt',
            'face' => 'Face',
            'head' => 'Head'
        ];

        $type = $typeNames[$item->type] ?? ucfirst($item->type);

        $itemImage = ($item->status === 'accepted')
            ? $item->thumbnail
            : '/images/reviewpending.png';

        $priceBux = (int)($item->price_bux ?? 0);
        $priceTix = (int)($item->price_tix ?? 0);

        $userBux = auth()->check()
            ? (int)(auth()->user()->bux ?? 0)
            : 0;

        $userTix = auth()->check()
            ? (int)(auth()->user()->tix ?? 0)
            : 0;

        return view('item', [
            'item' => $item,
            'creator' => $creator,
            'comments' => $comments,
            'favorites' => $favorites,
            'owned' => $owned,

            'type' => $type,
            'itemImage' => $itemImage,

            'priceBux' => $priceBux,
            'priceTix' => $priceTix,

            'userBux' => $userBux,
            'userTix' => $userTix
        ]);
    }
}