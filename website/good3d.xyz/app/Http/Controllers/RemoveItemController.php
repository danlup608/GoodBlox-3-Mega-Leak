<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use Illuminate\Support\Facades\DB;
use Illuminate\Support\Facades\Auth;

class RemoveItemController extends Controller
{
    public function remove(Request $request)
    {
        if (!Auth::check()) {
            return response('unauthorized', 401);
        }

        $uid = Auth::id();
        $itemId = (int) $request->input('id');

        if ($itemId <= 0) {
            return response('invalid_item', 400);
        }

        DB::table('wearing')
            ->where('userid', $uid)
            ->where('itemid', $itemId)
            ->delete();

        return response('ok');
    }
}