<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use Illuminate\Support\Facades\DB;

class InboxController extends Controller
{
    public function index(Request $request)
    {
        $messages = DB::table('messages')
            ->where('user_to', auth()->id())
            ->orderByDesc('id')
            ->paginate(20);

        return view('my.inbox', compact('messages'));
    }
}