<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use Illuminate\Support\Facades\DB;

class ServerController extends Controller
{
    public function ping(Request $request)
    {
        if ($request->query('accesskey') !== 'bloxrlcoolbloxrlv3rendereryep') {
            return response('No', 403);
        }

        DB::table('servers')
            ->where('serverId', 1)
            ->update([
                'serverping' => now()->timestamp,
            ]);

        return response('pinged', 200);
    }
}