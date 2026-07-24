<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use Illuminate\Support\Facades\DB;

class CatalogController extends Controller
{
    public function updateThumbnail(Request $request)
    {
        if ($request->query('accesskey') !== 'bloxrlcoolbloxrlv3rendereryep') {
            return response('no', 403);
        }

        $id = (int) $request->query('id', 0);
        $type = strtolower($request->query('type', ''));
        $thumb = $request->query('thumb', '');

        if (!$id || !$thumb) {
            return response('invalid', 400);
        }

        $validTypes = ['model', 'shirt', 'pants', 'head', 'hat'];

        if (!in_array($type, $validTypes)) {
            return response('skip', 200);
        }

        DB::table('catalog')
            ->where('id', $id)
            ->update([
                'thumbnail' => $thumb,
            ]);

        return response('ok', 200);
    }
}