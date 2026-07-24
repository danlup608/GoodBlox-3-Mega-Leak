<?php

namespace App\Http\Middleware;

use Closure;
use Illuminate\Http\Request;
use Symfony\Component\HttpFoundation\Response;
use Illuminate\Support\Facades\Auth;

class AdminMiddleware
{
    public function handle(Request $request, Closure $next): Response
    {
        $allowedRoles = ['Administrator', 'Moderator'];
        if (Auth::check() && in_array(Auth::user()->user_role, $allowedRoles)) {
            return $next($request);
        }
        return redirect('/')->with('error', 'You do not have staff permissions.');
    }
}