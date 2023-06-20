using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Reflection.Metadata;


namespace Lab3
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
    }

    public class Product
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
    }

    class Program
    {
        static readonly HttpClient client = new HttpClient();

        static async Task Main(string[] args)
        {

            Console.WriteLine("Welcome !\n");

            while (true)
            {
                Console.WriteLine("Select an option:");
                Console.WriteLine("----------------------------\n");
                Console.WriteLine("1. Display categories");
                Console.WriteLine("2. Show category details");
                Console.WriteLine("3. Add new category");
                Console.WriteLine("4. Delete category");
                Console.WriteLine("5. Update category title");
                Console.WriteLine("6. Add product to category");
                Console.WriteLine("7. Display products in category");
                Console.WriteLine("8. Quit");

                string option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        await DisplayCategories();
                        break;
                    case "2":
                        await ShowCategoryDetails();
                        break;
                    case "3":
                        AddCategory();
                        break;
                    case "4":
                        DeleteCategory();
                        break;
                    case "5":
                        UpdateCategoryName();
                        break;
                    case "6":
                        AddProductToCategory();
                        break;
                    case "7":
                        await DisplayProductsInCategory();
                        break;
                    case "8":
                        return;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
        }

        static async Task DisplayCategories()
        {
            Console.Clear();

            try
            {
                HttpResponseMessage response = await client.GetAsync("http://localhost:5000/api/Category/categories");
                response.EnsureSuccessStatusCode();

                List<Category> categoryList = await response.Content.ReadAsAsync<List<Category>>();

                Console.WriteLine("Categories:");
                foreach (var category in categoryList)
                {
                    Console.WriteLine($"{category.Id}: {category.Name}");
                }

                Console.WriteLine("");
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Error retrieving categories: {e.Message}");
            }
        }

        static async Task ShowCategoryDetails()
        {
            Console.Clear();

            Console.WriteLine("Please enter the category ID:");
            int categoryId = int.Parse(Console.ReadLine());

            try
            {
                HttpResponseMessage response = await client.GetAsync($"http://localhost:5000/api/Category/categories/{categoryId}");
                response.EnsureSuccessStatusCode();

                List<Category> categories = await response.Content.ReadAsAsync<List<Category>>();

                foreach (var category in categories)
                {
                    Console.WriteLine($"Category details for ID {category.Id}:");
                    Console.WriteLine($"Name: {category.Name}");
                    Console.WriteLine($"Items count: {category.Count}");
                    Console.WriteLine("");
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Error retrieving category details: {e.Message}");
            }
        }
        static async void AddCategory()
        {
            Console.Clear();

            Console.WriteLine("Enter the name of the new category:");
            var title = Console.ReadLine();

            using (var httpClient = new HttpClient())
            {
                var newCategory = new { title };

                var json = JsonSerializer.Serialize(newCategory);

                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");              

                try
                {
                    await httpClient.PostAsync("https://localhost:5001/api/Category/categories", content);
                }

                catch (HttpRequestException ex)
                {
                    Console.WriteLine($"Failed to create category '{title}'.");
                }
            }
        }

        static async void DeleteCategory()
        {
            Console.Clear();

            Console.WriteLine("Enter the ID of the category you want to delete:");
            int id = int.Parse(Console.ReadLine());

            using (var httpClient = new HttpClient())
            {
                
                try
                {
                    await httpClient.DeleteAsync($"https://localhost:5001/api/Category/categories/{id}");
                }

                catch (HttpRequestException ex)
                {
                    Console.WriteLine($"Failed to delete category with ID '{id}'.");
                }
            }
        }

        static async void UpdateCategoryName()
        {
            Console.Clear();

            Console.WriteLine("Enter the ID of the category to update:");
            var id = Console.ReadLine();

            Console.WriteLine("Enter the new title:");
            var title = Console.ReadLine();

            using (var httpClient = new HttpClient())
            {
                var newTitle = new { title };

                var json = JsonSerializer.Serialize(newTitle);

                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
  
                try
                {
                    await httpClient.PutAsync($"https://localhost:5001/api/Category/{id}", content);
                }

                catch (HttpRequestException ex)
                {
                    Console.WriteLine($"Failed to update category '{id}' title to '{title}'.");
                }


            }
        }
        static async void AddProductToCategory()
        {
            Console.Clear();

            Console.WriteLine("Enter the ID of the category:");
            var categoryId = Console.ReadLine();

            Console.WriteLine("Enter the title of the new product:");
            var title = Console.ReadLine();

            Console.WriteLine("Enter the price of the new product:");
            var price = Console.ReadLine();

            var newProduct = new
            {
                id = 0,
                title,
                price,
                categoryId
            };

            using (var httpClient = new HttpClient())
            {
                var json = JsonSerializer.Serialize(newProduct);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                try
                {
                    await httpClient.PostAsync($"https://localhost:5001/api/Category/categories/{categoryId}/products", content);
                }

                catch (HttpRequestException ex) {
                    Console.WriteLine($"Failed to add product '{title}' to category with ID {categoryId}.");
                }
                
            }
        }
        static async Task DisplayProductsInCategory()
        {
            Console.Clear();

            Console.WriteLine("Enter the ID of the category to list products for:");
            var categoryId = Console.ReadLine();

            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync($"https://localhost:5001/api/Category/categories/{categoryId}/products");

                try
                {
                    var products = await response.Content.ReadAsAsync<IEnumerable<Product>>();

                    if (products.Any())
                    {
                        Console.WriteLine($"Products in category {categoryId}:");
                        foreach (var product in products)
                        {
                            Console.WriteLine($"{product.Id} - {product.Title} - {product.Price}");
                        }
                        Console.WriteLine("");
                    }
                }

                catch (HttpRequestException ex)
                {
                    Console.WriteLine($"Failed to display products for category {categoryId}.");
                }

                
            }
        }
    }

}
