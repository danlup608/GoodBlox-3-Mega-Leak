<?php

namespace App\Models;

use Illuminate\Foundation\Auth\User as Authenticatable;

class User extends Authenticatable
{
    protected $table = 'users';

    protected $primaryKey = 'id';

    public $timestamps = false;

    protected $fillable = [

    'username',
    'email',
    'password',
    'tix',
    'bux',
    'referral',
    'description',
];

    protected $hidden = [
        'password',
        'twofa_code'
    ];

    public function getAuthPassword()
    {
        return $this->password;
    }
}