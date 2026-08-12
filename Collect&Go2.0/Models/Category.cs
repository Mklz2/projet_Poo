using Collect_Go2._0.Interfaces;

namespace Collect_Go2._0.Models
{
    public class Category
    {
        private int categoryId;
        private string name = string.Empty;

        public int CategoryId
        {
            get => categoryId;
            init
            {
                if (value < 0)
                    throw new ArgumentException(
                        "L'identifiant de la catégorie ne peut pas être négatif.");

                categoryId = value;
            }
        }

        public string Name
        {
            get => name;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException(
                        "Le nom de la catégorie ne peut pas être vide.");

                name = value;
            }
        }

        public List<Product> Products { get; set; } = new();

        public Category()
        {
        }

        public Category(int categoryId, string name)
        {
            CategoryId = categoryId;
            Name = name;
        }

        public async Task LoadProductsAsync(IProductDAL productDal)
        {
            Products = await productDal.GetByCategoryAsync(CategoryId);
        }

        public static Task<List<Category>> GetAllAsync(ICategoryDAL categoryDal)
        {
            return categoryDal.GetAllAsync();
        }

        public static Task<Category?> GetByIdAsync(int categoryId, ICategoryDAL categoryDal)
        {
            return categoryDal.GetByIdAsync(categoryId);
        }
    }
}