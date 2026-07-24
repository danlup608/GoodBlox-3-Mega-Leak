<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;

class InviteKey extends Model
{
    protected $table = 'invite_keys';

    protected $fillable = [
        'invite_key',
        'used'
    ];

    public $timestamps = false;
}