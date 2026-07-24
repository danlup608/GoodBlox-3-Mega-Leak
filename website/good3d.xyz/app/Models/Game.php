<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;
use Illuminate\Database\Eloquent\Relations\BelongsTo;
use Illuminate\Database\Eloquent\Relations\HasMany;

class Game extends Model
{
    protected $table = 'games';
    public $timestamps = false;
    protected $casts = [
        'created' => 'datetime',
    ];
    public function creator(): BelongsTo
    {
        return $this->belongsTo(User::class, 'creatorid');
    }
    public function favorites(): \Illuminate\Database\Eloquent\Relations\HasMany
{
    return $this->hasMany(Favorite::class, 'itemid', 'id');
}
    public function servers(): HasMany
    {
        return $this->hasMany(GameServer::class, 'game_id');
    }
}