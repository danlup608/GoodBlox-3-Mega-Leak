<?php

namespace App\Models;
use Illuminate\Database\Eloquent\Model;
use Illuminate\Database\Eloquent\Relations\BelongsTo;

class Favorite extends Model
{
    protected $table = 'favorites';
    public $timestamps = false;

    protected $casts = [
        'whenFavorited' => 'datetime',
    ];
    public function game(): BelongsTo
    {
        return $this->belongsTo(Game::class, 'itemid', 'id');
    }
}