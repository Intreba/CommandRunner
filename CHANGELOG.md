## 2.4.0 (06-09-2017)
  * Update to netstandard20
## 2.3.2 (15-08-2017)
  * Add inner exception stack trace when executing command
## 2.3.1 (15-08-2017)
  * Ignore get/set methods on public type scanning
## 2.3.0 (15-08-2017)
  * Methods with similar call structure are now fixed (#2)
  * Auto up after commands is now available (#3)
  * Async void commands will show an error message and won't execute
  * Ability to scan public types, allowing modes where dependant libraries won't need a reference to commanr runner to still offer commands (#7)