<?php
use Illuminate\Support\Facades\Route;

Route::post('/renderserver/upload', function () {

    if (request()->query('accesskey') !== 'bloxrlcoolbloxrlv3rendereryep') {
        return response('No', 403);
    }

    if (!request()->hasFile('file')) {
        return response('upload error', 400);
    }

    $file = request()->file('file');

    if (!$file->isValid()) {
        return response('upload error', 400);
    }

    $name = $file->getClientOriginalName();

    $path = public_path('renders');

    if (!file_exists($path)) {
        mkdir($path, 0777, true);
    }

    $file->move($path, $name);

    return response('uploaded', 200);
});
?>