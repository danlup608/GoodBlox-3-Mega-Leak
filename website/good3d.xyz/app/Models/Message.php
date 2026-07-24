<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;

class Message extends Model
{
    protected $table = 'messages';

    protected $primaryKey = 'id';

    public $timestamps = false;

    protected $fillable = [

        'user_from',
        'user_to',
        'subject',
        'content',
        'datesent',
        'readfrom',
        'readto',
        'deletefrom',
        'deleteto',
    ];
}