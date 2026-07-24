<?php namespace App\Http\Controllers;

use Illuminate\Http\Request;
use App\Models\User;

class PeopleController extends Controller
{
    public function index(Request $request)
    {
        $resultsperpage = 10;

        $searchbar = trim($request->input('search', ''));

        $query = User::query();

        if ($searchbar !== '') {
            $query->where('username', 'LIKE', "%{$searchbar}%");
        }

        $query->orderByDesc('lastseen');

        $users = $query->paginate($resultsperpage);

        return view('browse', [
            'users' => $users,
            'searchbar' => $searchbar
        ]);
    }
}