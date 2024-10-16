using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using SalesApi;  
using SalesApi.Domain.Entities;
using Xunit;

public class VendasControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    public readonly HttpClient _client;

    private VendasControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CRUDFlow_ShouldWorkAsExpected()
    {
        // Criação da venda
        var venda = new Venda
        {
            NumeroVenda = "12345",
            DataVenda = DateTime.UtcNow,
            ClienteId = Guid.NewGuid(),
            ClienteNome = "Cliente Teste",
            Filial = "Filial X",
            ValorTotal = 150.0m,
            Itens = new List<ItemVenda>
            {
                new ItemVenda { Id = Guid.NewGuid(), ProdutoNome = "Produto A", Quantidade = 1, ValorUnitario = 100, Desconto = 0 }
            }
        };

        var createResponse = await _client.PostAsJsonAsync("/api/vendas", venda);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdVenda = await createResponse.Content.ReadFromJsonAsync<Venda>();
        createdVenda.Should().NotBeNull();
        createdVenda!.Id.Should().NotBeEmpty();

        // Consultar a venda
        var getResponse = await _client.GetAsync($"/api/vendas/{createdVenda.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var fetchedVenda = await getResponse.Content.ReadFromJsonAsync<Venda>();
        fetchedVenda.Should().BeEquivalentTo(createdVenda);

        // Atualizar a venda
        fetchedVenda.Itens.Add(new ItemVenda { Id = Guid.NewGuid(), ProdutoNome = "Produto B", Quantidade = 2, ValorUnitario = 50, Desconto = 5 });
        var updateResponse = await _client.PutAsJsonAsync($"/api/vendas/{fetchedVenda.Id}", fetchedVenda);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Cancelar um item
        var itemToCancel = fetchedVenda.Itens[0];
        var cancelItemResponse = await _client.DeleteAsync($"/api/vendas/{fetchedVenda.Id}/itens/{itemToCancel.Id}");
        cancelItemResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verificar se o item foi cancelado
        getResponse = await _client.GetAsync($"/api/vendas/{fetchedVenda.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var vendaAfterCancellation = await getResponse.Content.ReadFromJsonAsync<Venda>();
        vendaAfterCancellation.Itens.Should().NotContain(i => i.Id == itemToCancel.Id);

        // Remover a venda
        var deleteResponse = await _client.DeleteAsync($"/api/vendas/{fetchedVenda.Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verificar se a venda foi removida
        var verifyDeleteResponse = await _client.GetAsync($"/api/vendas/{fetchedVenda.Id}");
        verifyDeleteResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
