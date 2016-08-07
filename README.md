DotNetRandomNameGenerator
=========================

Generates random people and place names drawn from freely available US census data.

[![Join the chat at https://gitter.im/m4bwav/DotNetRandomNameGenerator](https://badges.gitter.im/m4bwav/DotNetRandomNameGenerator.svg)](https://gitter.im/m4bwav/DotNetRandomNameGenerator?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

Nuget package: https://www.nuget.org/packages/RandomNameGeneratorLibrary/

The library contains a stripped down lists of human names from the US Census names list, and a list of place names from another census list. The library allows you to get random first and last names or both and you can get male and female first names. You can also generate random place names as well. To access this functionality create a NameGenerator in namespace RandomNameGenerator, and call one of the functions like GenerateRandomFirstAndLastName(). The functions names describe literally and simply what those functions do.

Typical Usage (getting a person name):  
    var personGenerator = new PersonNameGenerator()
	var name = _personGenerator.GenerateRandomFirstAndLastName();
	
	Console.WriteLine(name); //Outputs some random first and last name
