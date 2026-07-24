<?php
header('Content-type: text/javascript');

if (!$_GET['accesskey']) { die("No"); }
if ($_GET['accesskey'] != "bloxrlcoolbloxrlv3rendereryep") { die("No"); }

include $_SERVER['DOCUMENT_ROOT']."/maincore/config.php";

$repoSQL = $db;

$row = $repoSQL->query("SELECT `jobId`, `renderType`, `renderData`, `targetId`, `targetType`
FROM renderqueue 
WHERE `renderStatus` = 0 OR `renderStatus` = 1 
ORDER BY `jobId` ASC LIMIT 1")->fetch(PDO::FETCH_OBJ);

$rowcount = $repoSQL->query("SELECT count(*) FROM renderqueue WHERE `renderStatus` = 0")->fetchColumn();

if (!$rowcount){ 
    die("fart"); 
} else { 
    echo json_encode($row, JSON_PRETTY_PRINT); 
}

$query = $repoSQL->prepare('UPDATE renderqueue 
SET `renderStatus` = 1, `timestampAcknowledged` = UNIX_TIMESTAMP() 
WHERE `jobId` = :jobid');

$query->bindValue(':jobid', $row->jobId, PDO::PARAM_INT);
$query->execute();
?>