<?php 
  if (!isset($_GET['accesskey'])) { die("No"); }
  if ($_GET['accesskey'] != "bloxrlcoolbloxrlv3rendereryep") { die("No"); }
  header('Content-Type: text/plain; charset=utf-8'); 
  if(isset($_GET['signature']) && $_GET['signature'] == 'true') { ob_start(); }

  echo $_GET['script'];

  if(isset($_GET['signature']) && $_GET['signature'] == 'true') 
  { 
    $script = ob_get_clean();
    $privatekey = openssl_pkey_get_private("file://private_key.pem");
    openssl_sign($script, $signature, $privatekey);
    echo "%" . base64_encode($signature) . "%" . $script;
  }

?>
