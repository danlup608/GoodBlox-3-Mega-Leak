<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use Illuminate\Support\Facades\DB;

class PrivateMessageController extends Controller
{
    public function show(Request $request)
    {
        $id = (int) $request->query('MessageID');

        if ($id <= 0) {
            abort(404, 'Invalid message ID');
        }

        $message = DB::table('messages')
            ->where('id', $id)
            ->first();

        if (!$message) {
            abort(404, 'Message not found');
        }

        if ($message->user_to != auth()->id()) {
            abort(403, 'Not your message');
        }

        $author = DB::table('users')
            ->select('id', 'username')
            ->where('id', $message->user_from)
            ->first();

        // mark as read
        DB::table('messages')
            ->where('id', $id)
            ->update(['readto' => 1]);

        return view('my.message', [
            'message' => $message,
            'author' => $author,
            'sitename' => config('app.name')
        ]);
    }
}