<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use Illuminate\Support\Facades\DB;
use Illuminate\Support\Facades\Auth;

class WearItemController extends Controller
{
    public function wear(Request $request)
    {
        if (!Auth::check()) {
            return response('unauthorized', 401);
        }

        $uid = Auth::id();
        $itemId = (int) $request->input('id');

        if (!$itemId) {
            return response('invalid_item', 400);
        }

        $item = DB::table('catalog')
            ->where('id', $itemId)
            ->first();

        if (!$item) {
            return response('not_found', 404);
        }

        if ($item->type === 'hat') {

            $hatCount = DB::table('wearing as w')
                ->join('catalog as c', 'w.itemid', '=', 'c.id')
                ->where('w.userid', $uid)
                ->where('c.type', 'hat')
                ->count();

            if ($hatCount >= 3) {
                return response('max_hats', 400);
            }
        }

        if ($item->type !== 'hat') {

            DB::table('wearing as w')
                ->join('catalog as c', 'w.itemid', '=', 'c.id')
                ->where('w.userid', $uid)
                ->where('c.type', $item->type)
                ->delete();
        }

        DB::table('wearing')->updateOrInsert(
            [
                'userid' => $uid,
                'itemid' => $itemId
            ],
            []
        );

        return response('ok');
    }
}