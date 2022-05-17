<?php

	$connection = mysqli_connect('localhost:3307', 'root', 'root', 'magic_survival');

	// check connection state
	if (mysqli_connect_errno()) 
	{
		echo "1";
		exit();
	}

	$username = $_POST["username"];
	$level = $_POST["level"];
	$xp = $_POST["experience"];
	$currency1 = $_POST["currency1"];
	$currency2 = $_POST["currency2"];

	// check name existance
	$namecheckquery = "SELECT username FROM accounts WHERE username='" . $username . "';";

	$namecheck = mysqli_query($connection, $namecheckquery) or die("2");

	if (mysqli_num_rows($namecheck) != 1)
	{
		echo "5";
		exit();
	}

	$updateQuery = "UPDATE accounts SET level = " . $level . " WHERE username = '" . $username . "';";
	mysqli_query($connection, $updateQuery) or die("7");

	$updateQuery = "UPDATE accounts SET experience = " . $xp . " WHERE username = '" . $username . "';";
	mysqli_query($connection, $updateQuery) or die("8");

	$updateQuery = "UPDATE accounts SET currency1 = " . $currency1 . " WHERE username = '" . $username . "';";
	mysqli_query($connection, $updateQuery) or die("9");

	$updateQuery = "UPDATE accounts SET currency2 = " . $currency2 . " WHERE username = '" . $username . "';";
	mysqli_query($connection, $updateQuery) or die("10");

	echo "0";


?>