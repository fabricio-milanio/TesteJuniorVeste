using TesteVeste.Application.DTOs;
using TesteVeste.Application.Interfaces;
using TesteVeste.Application.Notifications;
using TesteVeste.Domain.Entities;
using TesteVeste.Domain.Interfaces;
using TesteVeste.Domain.Shared;

namespace TesteVeste.Application.Services;

// =============================================================================
//  TODO — SUA TAREFA
// =============================================================================
//  Implemente todos os métodos desta classe seguindo as regras de negócio abaixo.
//  Consulte o CategoriaService.cs como referência de implementação.
//
//  REGRAS DE NEGÓCIO:
//  [1] Nome é obrigatório e deve ter no máximo 100 caracteres.
//  [2] Preço deve ser maior que zero.
//  [3] Não pode existir dois produtos com o mesmo nome (ignorar capitalização).
//  [4] Um produto INATIVO não pode ser editado (UpdateAsync deve falhar).
//  [5] O DeleteAsync é um "soft delete": apenas define Ativo = false.
//
//  DICA: Use _notifications.AddNotification("mensagem") para registrar erros
//  e retorne CommandResult<T>.Failure(_notifications.Notifications) em caso de falha.
// =============================================================================

public class ProdutoService : IProdutoService
{
    private readonly IProdutoRepository _repository;
    private readonly INotificationService _notifications;

    public ProdutoService(IProdutoRepository repository, INotificationService notifications)
    {
        _repository = repository;
        _notifications = notifications;
    }
    
    public async Task<CommandResult<PagedResult<ProdutoDto>>> GetAllAsync(int pagina, int tamanhoPagina)
    {
        var pagedResult = await _repository.GetAllAsync(pagina, tamanhoPagina);
        
        var dtos = pagedResult.Itens.Select(MapToDto).ToList();
        
        var pagedDtoResult = new PagedResult<ProdutoDto>
        {
            Pagina = pagedResult.Pagina,
            TamanhoPagina = pagedResult.TamanhoPagina,
            TotalItens = pagedResult.TotalItens,
            Itens = dtos
        };
        
        return CommandResult<PagedResult<ProdutoDto>>.Success(pagedDtoResult);
    }

    public async Task<CommandResult<ProdutoDto>> GetByIdAsync(int id)
    {
        // TODO: Busque o produto pelo Id.
        //       Se não existir, adicione uma notificação e retorne Failure.
        var product = await  _repository.GetByIdAsync(id);

        if (product == null)
        {
            _notifications.AddNotification("Produto não encontrado");
            return CommandResult<ProdutoDto>.Failure(_notifications.Notifications);
        }
        
        var productDto =  MapToDto(product);
        
        return CommandResult<ProdutoDto>.Success(productDto);
    }

    public async Task<CommandResult<ProdutoDto>> CreateAsync(CreateProdutoDto dto)
    {
        // TODO: Valide os campos (regras 1 e 2), verifique duplicidade de nome (regra 3)
        //       e persista o novo produto.
        
        if (string.IsNullOrWhiteSpace(dto.Nome))
        {
            _notifications.AddNotification("O nome do produto é obrigatório.");
        }
        else if (dto.Nome.Length > 100)
        {
            _notifications.AddNotification("O nome do produto deve ter no máximo 100 caracteres.");
        }
        
        if (dto.Preco <= 0)
        {
            _notifications.AddNotification("O preço do produto deve ser maior que zero.");
        }
        
        if (_notifications.HasNotifications)
        {
            return CommandResult<ProdutoDto>.Failure(_notifications.Notifications);
        }
        
        var duplicateName = await _repository.ExistsWithNameAsync(dto.Nome);
        if (duplicateName)
        {
            _notifications.AddNotification("Já existe um produto cadastrado com este nome.");
            return CommandResult<ProdutoDto>.Failure(_notifications.Notifications);
        }
        
        var product = new Produto
        {
            Nome = dto.Nome,
            Descricao = dto.Descricao,
            Preco = dto.Preco,
            Estoque = dto.Estoque,
            CategoriaId = dto.CategoriaId
        };

        await _repository.AddAsync(product);
        
        var success = await _repository.SaveChangesAsync();

        if (!success)
        {
            _notifications.AddNotification("Não foi possível salvar o produto.");
            return CommandResult<ProdutoDto>.Failure(_notifications.Notifications);
        }
        
        var productDto = MapToDto(product);
        
        return CommandResult<ProdutoDto>.Success(productDto);
    }

    public Task<CommandResult<ProdutoDto>> UpdateAsync(int id, UpdateProdutoDto dto)
    {
        // TODO: Busque o produto, valide se está ativo (regra 4),
        //       valide os campos (regras 1 e 2), verifique duplicidade (regra 3)
        //       e salve as alterações.
        throw new NotImplementedException();
    }

    public Task<CommandResult<bool>> DeleteAsync(int id)
    {
        // TODO: Busque o produto. Se não existir, retorne Failure.
        //       Caso contrário, defina Ativo = false e salve (regra 5).
        throw new NotImplementedException();
    }
    
    private static ProdutoDto MapToDto(Produto produto)
    {
        return new ProdutoDto
        {
            Id = produto.Id,
            Nome = produto.Nome,
            Descricao = produto.Descricao,
            Preco = produto.Preco,
            Estoque = produto.Estoque,
            Ativo = produto.Ativo,
            DataCadastro = produto.DataCadastro,
            CategoriaId = produto.CategoriaId,
            CategoriaNome = produto.Categoria?.Nome
        };
    }
}
