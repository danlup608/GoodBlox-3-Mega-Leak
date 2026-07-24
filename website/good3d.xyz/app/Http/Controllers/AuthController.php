<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use App\Models\User;
use App\Models\InviteKey;
use App\Models\Message;
use Illuminate\Support\Facades\Auth;
use Illuminate\Support\Facades\Hash;
use Illuminate\Support\Facades\DB;
use Illuminate\Support\Facades\Cookie;

class AuthController extends Controller
{


    public function showRegister()
    {
        return view('auth.register');
    }

    

    public function showLogin()
    {
        return view('auth.login');
    }



    public function register(Request $request)
{
    $badWords = [
        'fag', 'nigger', 'nigga', 'faggot', 'admin', 'moderator', 'staff', 'fuck', 'slut', 'porn', 'bitch', 'retard', 'retarded', 'gay', 'shit', 'cum', 'rule34', 'shitty', 'asshole', 'ass', 'pussy', 'vagina', 'anus', 'slutty', 'milf', 'gore', 'futa', 'yuri', 'pedophile', 'pedo', 'fisting', 'kys'
    ];

    $request->validate([
        'username' => [
            'required',
            'min:3',
            'max:20',
            'regex:/^[a-zA-Z0-9]+$/',
            'unique:users,username'
        ],
        'email' => [
            'required',
            'email',
            'unique:users,email'
        ],
        'password' => [
            'required',
            'min:8',
            'confirmed'
        ],
        'invitekey' => [
            'required'
        ]
    ], [
        'username.required' => 'Please enter a username.',
        'username.min' => 'Username must be at least 3 characters.',
        'username.max' => 'Username cannot exceed 20 characters.',
        'username.regex' => 'Username contains invalid characters.',
        'username.unique' => 'That username is already taken.',
        'email.required' => 'Please enter an email address.',
        'email.email' => 'Please enter a valid email address.',
        'email.unique' => 'That email is already registered.',
        'password.required' => 'Please enter a password.',
        'password.min' => 'Passwords must be at least 8 characters.',
        'password.confirmed' => 'Passwords do not match.',
        'invitekey.required' => 'Please enter an invite key.',
    ]);

    $lowercaseUsername = strtolower($request->username);
    foreach ($badWords as $badWord) {
        if (str_contains($lowercaseUsername, $badWord)) {
            return back()
                ->withErrors(['username' => 'This username contains inappropriate language or words that are not allowed.'])
                ->withInput();
        }
    }

    $invite = InviteKey::where(
        'invite_key',
        strtoupper(trim($request->invitekey))
    )->first();

    if (!$invite) {
        return back()
            ->withErrors(['invitekey' => 'Invalid invite key.'])
            ->withInput();
    }

    if ($invite->used == 1) {
        return back()
            ->withErrors(['invitekey' => 'That invite key has already been used.'])
            ->withInput();
    }

    $referralUser = 'None';
    if ($request->filled('referral')) {
        $referredBy = User::where('username', trim($request->referral))->first();
        if ($referredBy) {
            $referralUser = $referredBy->username;
        }
    }
  
    $user = User::create([
        'username' => trim($request->username),
        'email' => trim($request->email),
        'password' => Hash::make($request->password),
        'tix' => 15,
        'bux' => 0,
        'referral' => $referralUser,
        'description' => '',
    ]);

    $invite->used = 1;
    $invite->used_by = $user->id;
    $invite->save();

    $defaultRender = public_path('renders/user_0-420x420.png');
    $newRender = public_path('renders/user_' . $user->id . '-420x420.png');

    if (file_exists($defaultRender)) {
        copy($defaultRender, $newRender);
    }

    Message::create([
        'user_from' => 1,
        'user_to' => $user->id,
        'subject' => 'Dear GoodBlox User',
        'content' => 'Welcome to GoodBlox! We are constantly working to make GoodBlox a fun, safe, creative place for people of all ages. We update GoodBlox regularly, so be sure to visit our NEWS section to find out about all the exciting changes.

If you have questions about how something works, our HELP section is a great place to start. It is maintained by the GoodBlox community for the GoodBlox community. There is a ton of helpful information, including tutorials and answers to Frequently Asked Questions (FAQs).

Finally, please feel free to post your comments and suggestions in the GoodBlox forums.

Thank you!

(Mark)
Owner, GoodBlox',
        'readto' => 0,
        'datesent' => now(),
    ]);

    Auth::login($user);
    $this->generateSessionCookie($user->id);
    return redirect('/');
}

    public function login(Request $request)
    {
        $credentials = [

            'username' => $request->username,

            'password' => $request->password
        ];

        if (Auth::attempt($credentials)) {

            $request->session()->regenerate();

            $this->generateSessionCookie(Auth::id());

            return redirect('/');
        }

        return back()->withErrors([
            'username' => 'Invalid username or password.'
        ]);
    }



    public function logout(Request $request)
    {
        $token = request()->cookie('GOODBLOSECURITY');

        if ($token) {

            DB::table('goodsessions')
                ->where('sessKey', $token)
                ->delete();
        }

        Cookie::queue(
            Cookie::forget('GOODBLOSECURITY')
        );

        Auth::logout();

        $request->session()->invalidate();

        $request->session()->regenerateToken();

        return redirect('/');
    }

    private function generateSessionCookie($userId)
    {
        $token = bin2hex(random_bytes(32));

        DB::table('goodsessions')->insert([

            'sessKey' => $token,

            'userId' => $userId,

            'created' => time()
        ]);

        Cookie::queue(
            'GOODBLOSECURITY',
            $token,
            60 * 24 * 365,
            '/',
            null,
            false,
            true
        );

        return $token;
    }
}