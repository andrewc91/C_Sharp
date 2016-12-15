using Nancy;
using ApiCaller;

namespace PokeInfo
{
    public class PokeInfoModule : NancyModule
    {
        public PokeInfoModule()
        {
            Get("/", async args => 
            {
                string Name = "";

                await WebRequest.SendRequest("http://pokeapi.co/api/v2/pokemon/1", new Action<Dictionary<string, object>>(JsonResponse =>
                {
                    Name = (string)JsonResponse["name"];
                }
                ));
                return View["index"];
            });
        }
    }
}