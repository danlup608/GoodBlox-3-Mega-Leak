<?php

namespace App\Http\Middleware;

use Closure;
use Illuminate\Http\Request;
use Illuminate\Support\Facades\Auth;
use App\Models\User;
use App\Models\Message;

class GoodBloxSession
{
    public function handle(Request $request, Closure $next)
    {
        if (Auth::check()) {

            $user = Auth::user();


            if ($user->bantype !== 'None') {
                return redirect('/Membership/NotApproved.aspx');
            }

            $now = time();

            if ($now - $user->reward_tix >= 86400) {
                $user->tix += 15;
                $user->reward_tix = $now;
            }


            if ($user->buildersclub === '1') {
                if ($now - $user->reward_bucks >= 86400) {
                    $user->bux += 50;
                    $user->reward_bucks = $now;
                }
            }

            $user->lastseen = $now;

            $user->save();

            $messageCount = Message::where('user_to', $user->id)
                ->where('readto', 0)
                ->count();

            view()->share('messageCount', $messageCount);
        }

        return $next($request);
    }
}