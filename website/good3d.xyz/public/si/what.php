<?php
include $_SERVER['DOCUMENT_ROOT']."/maincore/config.php";
$repoSQL = $db;

$jobsDir = $_SERVER['DOCUMENT_ROOT']."/renders/jobs/";
$finalDir = $_SERVER['DOCUMENT_ROOT']."/renders/";

$files = glob($jobsDir . "*.png");

foreach ($files as $file) {

    $name = basename($file);

    // sacar jobId
    preg_match('/^([a-zA-Z0-9]+)/', $name, $m);
    $jobId = $m[1] ?? null;

    if (!$jobId) continue;

    // buscar en DB
    $query = $repoSQL->prepare("
        SELECT renderData 
        FROM renderqueue 
        WHERE responseData = :jobid 
        AND renderStatus = 2
        LIMIT 1
    ");
    $query->execute([':jobid' => $jobId]);
    $row = $query->fetch(PDO::FETCH_ASSOC);

    if (!$row) continue;

    $data = json_decode($row['renderData'], true);

    if (!isset($data['uid'])) continue;

    $uid = (int)$data['uid'];

    $newPath = $finalDir . "render_user_{$uid}.png";

    rename($file, $newPath);

    echo "Moved $name -> render_user_$uid.png\n";
}