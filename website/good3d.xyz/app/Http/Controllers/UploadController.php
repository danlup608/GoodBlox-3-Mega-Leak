<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use Illuminate\Support\Facades\Auth;
use Illuminate\Support\Facades\DB;

class UploadController extends Controller
{
    private $types = [
        0 => ['folder' => 'tshirts', 'type' => 'tshirt'],
        1 => ['folder' => 'shirts', 'type' => 'shirt'],
        2 => ['folder' => 'pants', 'type' => 'pants'],
        3 => ['folder' => 'decals', 'type' => 'decal'],
        4 => ['folder' => 'audio', 'type' => 'audio'],
        5 => ['folder' => 'models', 'type' => 'model'],
    ];

    public function index(Request $request)
    {
        $type = (int) $request->query('type', 0);

        if (!isset($this->types[$type])) {
            abort(404);
        }

        $typeData = $this->types[$type];

        $name = ucfirst($typeData['type']);

        $sitename = config('app.name');
        $siteurl = config('app.url');

        $explanation = "
        <p class='instruction-text'>
        On {$sitename}, a {$name} is a textured character adornment that is applied to all surfaces of the character's arms and torso.
        To create a {$name}:
        </p>

        <ol class='instruction-list'>
            <li>Open the <a href='{$siteurl}/templates/{$typeData['folder']}.png'>{$name} Template</a> in an image editor.</li>
            <li>Modify the template.</li>
            <li>Save the customized texture.</li>
            <li>Click the Browse button.</li>
            <li>Select your texture.</li>
            <li>Click Create {$name}.</li>
        </ol>

        <p class='instruction-text'>
        The texture will be uploaded to {$sitename} and added to your inventory.<br>
        To wear it, go to <a href='/My/Character.aspx'>Change Character</a>.
        </p>
        ";

        return view('upload', [
            'type' => $type,
            'typeData' => $typeData,
            'explanation' => $explanation
        ]);
    }

    public function upload(Request $request)
    {
        $type = (int) $request->query('type', 0);

        if (!isset($this->types[$type])) {
            abort(404);
        }

        $typeData = $this->types[$type];

        $request->validate([
            'file' => 'required|file|max:5120'
        ]);

        $file = $request->file('file');

        $original = $file->getClientOriginalName();

        if (str_contains($original, ' ')) {
            return back()->with('error', 'Filename cannot contain spaces.');
        }

        $ext = strtolower($file->getClientOriginalExtension());

        switch ($typeData['type']) {

            case 'shirt':
            case 'pants':
            case 'tshirt':
            case 'decal':

                $allowed = ['png', 'jpg', 'jpeg'];

                if (!in_array($ext, $allowed)) {
                    return back()->with('error', 'Invalid image format.');
                }

                break;

            case 'audio':

                $allowed = ['mp3', 'ogg', 'wav'];

                if (!in_array($ext, $allowed)) {
                    return back()->with('error', 'Invalid audio format.');
                }

                break;

            case 'model':

                $allowed = ['rbxm', 'xml'];

                if (!in_array($ext, $allowed)) {
                    return back()->with('error', 'Invalid model format.');
                }

                break;
        }

        $folder = public_path('images/items/' . $typeData['folder']);

        if (!file_exists($folder)) {
            mkdir($folder, 0777, true);
        }

        if (file_exists($folder . '/' . $original)) {
            return back()->with('error', 'File already exists.');
        }

        $file->move($folder, $original);

        $filename = '/images/items/' . $typeData['folder'] . '/' . $original;

        $name = pathinfo($original, PATHINFO_FILENAME);

        $itemId = DB::table('catalog')->insertGetId([
            'name' => $name,
            'description' => 'hey, i just uploaded this!',
            'type' => $typeData['type'],
            'creator_id' => Auth::id(),
            'creator_name' => Auth::user()->username,
            'filename' => $filename,
            'thumbnail' => '/images/items/' . $typeData['folder'] . '/' . $original,
            'price_bux' => 0,
            'price_tix' => 0,
            'currency' => 'free',
            'created_at' => time(),
        ]);

        $base = "http://bloxrl.lol";

        switch ($typeData['type']) {

            case 'shirt':

                $renderType = 'Shirt';
                $renderData = $base . "/asset/ShirtFetch.php?id=" . $itemId . "&v=" . time();

                break;

            case 'pants':

                $renderType = 'Pants';
                $renderData = $base . "/asset/PantsFetch.php?id=" . $itemId . "&v=" . time();

                break;

            case 'model':

                $renderType = 'Model';
                $renderData = $base . $filename;

                break;

            default:

                return redirect('/Item.aspx?ID=' . $itemId);
        }

        DB::table('renderqueue')->insert([
            'renderType' => $renderType,
            'renderData' => $renderData,
            'renderStatus' => 0,
            'responseData' => '',
            'timestampCreated' => time(),
            'targetId' => $itemId,
            'targetType' => strtolower($renderType),
        ]);

        return redirect('/Item.aspx?ID=' . $itemId);
    }
}