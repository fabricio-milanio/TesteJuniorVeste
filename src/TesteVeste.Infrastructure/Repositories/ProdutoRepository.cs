using Microsoft.EntityFrameworkCore;
using TesteVeste.Domain.Entities;
using TesteVeste.Domain.Interfaces;
using TesteVeste.Domain.Shared;
using TesteVeste.Infrastructure.Data;

namespace TesteVeste.Infrastructure.Repositories;

// =============================================================================
//  TODO — SUA TAREFA
// =============================================================================
//  Implemente todos os métodos deste repositório usando Entity Framework Core.
//  Consulte o CategoriaRepository.cs como referência de implementação.
//
//  DICAS:
//  - Use _context.Produtos para acessar a tabela de produtos.
//  - Use .Include(p => p.Categoria) para carregar a categoria junto com o produto.
//  - Use .AsNoTracking() em consultas de leitura.
//  - Use Skip/Take para paginação: Skip((pagina - 1) * tamanhoPagina).Take(tamanhoPagina)
//  - Para ExistsWithNameAsync, compare nomes ignorando maiúsculas/minúsculas.
// =============================================================================

public class ProdutoRepository : IProdutoRepository
{
    private readonly AppDbContext _context;

    public ProdutoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<Produto>> GetAllAsync(int pagina, int tamanhoPagina)
    {
        // TODO: Retorne os produtos paginados incluindo a Categoria.
        //       Construa um PagedResult<Produto> com Pagina, TamanhoPagina, TotalItens e Itens.
        var query = _context.Produtos
            .Include(produto => produto.Categoria)
            .AsNoTracking();
        
        var totalItens = await query.CountAsync();

        var itens = await query
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToListAsync();
        
        return new PagedResult<Produto>
        {
            Itens = itens,
            TotalItens = totalItens,
            Pagina = pagina,
            TamanhoPagina = tamanhoPagina
        };
    }

    public async Task<Produto?> GetByIdAsync(int id)
    {
        // TODO: Retorne o produto pelo Id incluindo a Categoria.
        //       Retorne null se não encontrado.
        var product = await _context.Produtos
            .Include(produto => produto.Categoria)
            .FirstOrDefaultAsync(produto => produto.Id == id);

        return product;
    }

    public Task<bool> ExistsWithNameAsync(string nome, int? excludeId = null)
    {
        // TODO: Retorne true se já existir um produto com o mesmo nome.
        //       Ignore o produto com Id == excludeId (usado ao atualizar).
        throw new NotImplementedException();
    }

    public Task AddAsync(Produto produto)
    {
        // TODO: Adicione o produto ao contexto (sem salvar ainda).
        throw new NotImplementedException();
    }

    public void Update(Produto produto)
    {
        // TODO: Marque o produto como modificado no contexto (sem salvar ainda).
        throw new NotImplementedException();
    }

    public Task<bool> SaveChangesAsync()
    {
        // TODO: Salve as alterações e retorne true se ao menos uma linha foi afetada.
        throw new NotImplementedException();
    }
}
