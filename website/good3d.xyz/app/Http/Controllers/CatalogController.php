<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use Illuminate\Support\Facades\DB;

class CatalogController extends Controller
{
    public function index(Request $request)
    {
        $type = strtolower($request->query('type', 'hat'));
        $mode = $request->query('m', 'RecentlyUpdated');
        $page = max((int)$request->query('page', 1), 1);

        $perPage = 12;

        $query = DB::table('catalog');


        switch ($type) {

            case 'hat':
                $query->where('type', 'hat');
                $typeName = 'Hats';
                break;

            case 'shirt':
                $query->where('type', 'shirt');
                $typeName = 'Shirts';
                break;

            case 'pants':
                $query->where('type', 'pants');
                $typeName = 'Pants';
                break;

            case 'tshirt':
                $query->where('type', 'tshirt');
                $typeName = 'T-Shirts';
                break;

            case 'face':
                $query->where('type', 'face');
                $typeName = 'Faces';
                break;

            case 'head':
                $query->where('type', 'head');
                $typeName = 'Heads';
                break;

            default:
                $typeName = 'Catalog';
                break;
        }


        switch ($mode) {

            case 'BestSelling':
                $query->orderBy('sales', 'desc');
                $modeName = 'Best Selling';
                break;

            case 'TopFavorites':
                $query->orderBy('favorites', 'desc');
                $modeName = 'Top Favorites';
                break;

            case 'ForSale':
                $query->where('currency', '!=', 'free');
                $query->orderBy('id', 'desc');
                $modeName = 'For Sale';
                break;

            default:
                $query->orderBy('id', 'desc');
                $modeName = 'Recently Updated';
                break;
        }

        $total = $query->count();

        $pages = max(ceil($total / $perPage), 1);

        $items = $query
            ->skip(($page - 1) * $perPage)
            ->take($perPage)
            ->get();

        return view('catalog', [
            'items' => $items,
            'type' => $type,
            'typeName' => $typeName,
            'mode' => $mode,
            'modeName' => $modeName,
            'page' => $page,
            'pages' => $pages
        ]);
    }
}