<?php

	$connection = mysqli_connect('localhost:3307', 'root', 'root', 'magic_survival');

	// check connection state
	if (mysqli_connect_errno()) 
	{
		echo "1";
		exit();
	}

	$username = $_POST["username"];
	$password = $_POST["password"];

	// check name availability
	$namecheckquery = "SELECT username FROM accounts WHERE username='" . $username . "';";

	$namecheck = mysqli_query($connection, $namecheckquery) or die("2");

	if (mysqli_num_rows($namecheck) > 0)
	{
		echo "3";
		exit();
	}

	// Add new account to Table
	$salt = "\$5\$rounds=5000\$" . "markoaleksa" . $username . "\$";  // SHA256
	$hash = crypt($password, $salt);

	$insertquery = "INSERT INTO accounts (username, hash, salt) VALUES ('" . $username . "', '" . $hash . "', '" . $salt . "');";
	mysqli_query($connection, $insertquery) or die("4");

	echo("0");



?>