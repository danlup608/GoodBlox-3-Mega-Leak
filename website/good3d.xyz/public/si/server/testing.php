<?php

include $_SERVER['DOCUMENT_ROOT']."/maincore/config.php";

$repoSQL = $db;

$renderType = "Pants";

$assetId = "http://good3d.xyz/asset/PantsFetch.php?id=2";

/* 🧪 debug opcional */
# echo $assetId; exit;

$query = $repoSQL->prepare("
    INSERT INTO renderqueue 
    (renderType, renderData, renderStatus, timestampCreated)
    VALUES 
    (:type, :data, 0, :time)
");

$query->bindValue(':type', $renderType, PDO::PARAM_STR);
$query->bindValue(':data', $assetId, PDO::PARAM_STR);
$query->bindValue(':time', time(), PDO::PARAM_INT);

$query->execute();

echo "queued";