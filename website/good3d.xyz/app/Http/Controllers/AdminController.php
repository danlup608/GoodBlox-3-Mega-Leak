<?php

namespace App\Http\Controllers;
use \App\Models\User;
use Illuminate\Support\Facades\DB;
use Illuminate\Http\Request;
use Illuminate\Support\Facades\Http;

class AdminController extends Controller
{
    public function index()
    {
        $userCount = User::count();
        $gameCount = DB::table('games')->count();
        return view('admin.dash', compact('userCount', 'gameCount'));
    }
  
 public function usersList(Request $request)
{
    $search = $request->input('search', '');
    if (!empty($search)) {
        $users = User::where('username', 'LIKE', "%{$search}%")->paginate(25);
    } else {
        $users = User::paginate(25);
    }
    if (!empty($search)) {
        $users->appends(['search' => $search]);
    }
    return view('admin.users', compact('users', 'search')); 
}
  
    public function manageUsers(Request $request)
    {
        $search = $request->input('search');
        $query = User::orderBy('id', 'asc');
        if (!empty($search)) {
            $query->where(function($q) use ($search) {
                $q->where('id', $search)
                  ->orWhere('username', 'LIKE', '%' . $search . '%');
            });
        }
        $users = $query->paginate(20)->withQueryString();
        return view('admin.users', compact('users', 'search'));
    }

    public function viewUser($id)
    {
        $user = User::findOrFail($id);
        return view('admin.view_user', compact('user'));
    }
  
  public function scrubUsername($id)
{
    $user = User::findOrFail($id);
    $user->username = '[ Content Deleted ]';

    if (isset($user->description)) { $user->description = '[ Content Deleted ]'; }
    if (isset($user->blurb)) { $user->blurb = ''; }
    
    $user->save();
    return redirect()->back()->with('success', 'Username has been changed to [ Content Deleted ]');
}

public function updateRole(Request $request, $id)
{
    $request->validate([
        'role' => 'required|string',
    ]);

    $incomingRole = trim($request->input('role'));
    if (in_array(strtolower($incomingRole), ['user', 'username', 'none', ''])) {
        $finalRole = 'None';
    } elseif (str_contains(strtolower($incomingRole), 'admin')) {
        $finalRole = 'Administrator';
    } elseif (strtolower($incomingRole) === 'moderator') {
        $finalRole = 'Moderator';
    } else {
        $finalRole = 'None';
    }
    DB::update(
        "UPDATE users SET user_role = ? WHERE id = ?", 
        [$finalRole, (int)$id]
    );
    return redirect()->back()->with('success', 'User role updated successfully!');
}
  
public function updateCurrency(Request $request, $id)
{
    $user = User::findOrFail($id);
    if ($request->has('bux')) {
        $column = isset($user->reward_bucks) ? 'reward_bucks' : 'bux';
        $user->$column = (int)$request->input('bux');
    }
    
    if ($request->has('tix')) {
        $user->tix = (int)$request->input('tix');
    }
    
    $user->save();
    return redirect()->back()->with('success', 'Currency balance updated.');
}
  
  public function banUser(Request $request, $id)
{
    $user = User::findOrFail($id);
    $user->bantype = $request->input('bantype');
    $user->bannote = $request->input('bannote');
    $user->bandate = now();
    $user->save();
    return redirect()->back()->with('success', 'User has been banned.');
}

public function resetPassword($id)
{
    $user = User::findOrFail($id);
    $newPassword = Str::random(10);
    $user->password = Hash::make($newPassword);
    $user->save();
    return redirect()->back()->with('success', "Password successfully reset! New temporary string: {$newPassword}");
}

public function resetEmail($id)
{
    $user = User::findOrFail($id);
    $user->email = "reset-" . Str::random(5) . "@good3d.xyz";
    $user->save();
    return redirect()->back()->with('success', "Email asset field updated to: {$user->email}");
}
    public function pendingAssets()
    {
        $assets = DB::table('catalog')
            ->where('status', 'pending')
            ->orderBy('id', 'desc')
            ->get();
        return view('admin.pending_assets', compact('assets'));
    }
    public function moderateAsset(Request $request, $id)
    {
        $action = $request->input('action');
        $status = ($action === 'approve') ? 'accepted' : 'declined';

        DB::table('catalog')
            ->where('id', $id)
            ->update(['status' => $status]);
        return redirect()->back()->with('success', 'Asset status updated successfully');
    }
  public function manageAssets()
{
    $assets = DB::table('catalog')->orderBy('id', 'desc')->paginate(30);
    return view('admin.manage_assets', compact('assets'));
}

public function showCreateAsset()
{
    return view('admin.create_asset');
}

public function storeAsset(Request $request)
{
    $request->validate([
        'name' => 'required|string|max:100',
        'type' => 'required|in:shirt,pants,tshirt,face,hat,head,model',
        'price_bux' => 'nullable|integer|min:0',
        'price_tix' => 'nullable|integer|min:0',
        'currency' => 'required|in:free,bux,tix,both',
        'xml_file' => 'required|file',
        'image_file' => 'required|image|mimes:png,jpg,jpeg'
    ]);

    if ($request->hasFile('image_file')) {
        $imgFile = $request->file('image_file');
        $imgName = time() . '_' . $imgFile->getClientOriginalName();
        $imgFile->move(public_path('catalog/images'), $imgName);
        $filenamePath = '/catalog/images/' . $imgName;
    }

    DB::table('catalog')->insert([
        'name' => $request->name,
        'type' => $request->type,
        'filename' => $filenamePath,
        'price_bux' => $request->price_bux ?? 0,
        'price_tix' => $request->price_tix ?? 0,
        'currency' => $request->currency,
        'status' => 'accepted',
        'creator_id' => auth()->id(),
    ]);
    return redirect()->route('admin.manage.assets')->with('success', 'Asset Created successfully');
}

public function copyRobloxAsset(Request $request)
{
    $robloxAssetId = (int)$request->input('roblox_id');
    return redirect()->back()->with('success', 'Asset successfully imported into legacy system catalog');
}

public function manageGames()
{
    $games = DB::table('games')
        ->leftJoin('users', 'users.id', '=', 'games.creatorid')
        ->select('games.*', 'users.username as creator_name')
        ->orderBy('games.id', 'desc')
        ->get();
    return view('admin.manage_games', compact('games'));
}
  
public function deleteGame($id)
{
    DB::table('games')->where('id', $id)->delete();
    return redirect()->back()->with('success', 'Game removed successfully');
}
}