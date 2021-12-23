using TeamCity;
using TeamCity.BuildApi;

var endpoint = new Endpoint("eyJ0eXAiOiAiVENWMiJ9.NlRva3VBcEhUYlIxQkxIQm1ZZmZPNU1HUEJv.NjZlMmIxMTItYTRhNy00MTkzLWFlNmUtOGM2ZWI1NjRhZWRm", new Uri("http://localhost:8111/bs/"));
var teamCity = GetService<ITeamCity>().CreateClient(endpoint);
var builds = await teamCity.GetAsync(new GetAllBuilds());
WriteLine(builds.Count);