<?php
use Illuminate\Support\Facades\DB;
use Illuminate\Support\Facades\Route;

Route::get('/renderserver/complete', function () {

    if (request()->query('accesskey') !== 'bloxrlcoolbloxrlv3rendereryep') {
        return response('No', 403);
    }

    $type = (int) request()->query('type');
    $response = request()->query('response');

    if (!$type) {
        return response('type parameter not set', 400);
    }

    if (!$response) {
        return response('response parameter not set', 400);
    }

    $timestamp = now()->timestamp;

    switch ($type) {

        case 1:
            DB::table('renderqueue')
                ->where('renderStatus', 1)
                ->orderBy('jobId')
                ->limit(1)
                ->update([
                    'renderStatus' => 2,
                    'responseData' => $response,
                    'timestampCompleted' => $timestamp,
                ]);

            return response('success', 200);

        case 2:
            DB::table('renderqueue')
                ->where('renderStatus', 1)
                ->orderBy('jobId')
                ->limit(1)
                ->update([
                    'renderStatus' => 3,
                    'responseData' => $response,
                    'timestampCompleted' => $timestamp,
                ]);

            return response('success', 200);

        default:
            return response('invalid type', 400);
    }
});
?>