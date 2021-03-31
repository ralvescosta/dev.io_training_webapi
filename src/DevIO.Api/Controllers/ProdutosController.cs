using AutoMapper;
using DevIO.Api.Extensions;
using DevIO.Api.ViewModels;
using DevIO.Business.Intefaces;
using DevIO.Business.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DevIO.Api.Controllers
{
    [Route("api/produtos")]
    public class ProdutosController : MainController
    {
        private readonly IProdutoRepository _produtoRepository;
        private readonly IProdutoService _produtoService;
        private readonly IMapper _mapper;
        public ProdutosController(INotificador notificador, 
                                  IProdutoRepository produtoRepository, 
                                  IProdutoService produtoService, 
                                  IMapper mapper) : base(notificador)
        {
            _produtoRepository = produtoRepository;
            _produtoService = produtoService;
            _mapper = mapper;
        }

        [HttpPost]
        [ClaimsAuthorize("Fornecedor", "Adicionar")]
        public async Task<ActionResult<ProdutoViewModel>> Adicionar(ProdutoViewModel produtoViewModel)
        {
            if (!ModelState.IsValid)
                return CustomResponse(ModelState);

            var imagemNome = Guid.NewGuid() + "_" + produtoViewModel.Imagem;

            if (!UpdateArquivo(produtoViewModel.ImagemUpload, imagemNome))
            {
                return CustomResponse();
            }

            produtoViewModel.Imagem = imagemNome;
            await _produtoRepository.Adicionar(_mapper.Map<Produto>(produtoViewModel));

            return CustomResponse(produtoViewModel);
        }

        [RequestSizeLimit(40000000)]
        [HttpPost("adicionar")]
        [ClaimsAuthorize("Fornecedor", "Adicionar")]
        public async Task<ActionResult<ProdutoViewModel>> AdicionarAlternativo(ProdutoImagemViewModel produtoImagemViewModel)
        {
            if (!ModelState.IsValid)
                return CustomResponse(ModelState);

            var imagemPrefixo = Guid.NewGuid() + "_";

            if (!await UpdateArquivoAlternativo(produtoImagemViewModel.ImagemUpload, imagemPrefixo))
            {
                return CustomResponse();
            }

            produtoImagemViewModel.Imagem = imagemPrefixo + produtoImagemViewModel.ImagemUpload.FileName;
            await _produtoRepository.Adicionar(_mapper.Map<Produto>(produtoImagemViewModel));

            return CustomResponse(produtoImagemViewModel);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProdutoViewModel>>> ObterTodos()
        {
            var produtosViewModel = _mapper.Map<IEnumerable<ProdutoViewModel>>(await _produtoRepository.ObterProdutosFornecedores());
            return CustomResponse(produtosViewModel);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ProdutoViewModel>> ObterPorId(Guid id)
        {
            var produtoViewModel = _mapper.Map<ProdutoViewModel>(await _produtoRepository.ObterProdutoFornecedor(id));
            return CustomResponse(produtoViewModel);
        }

        [HttpPut("{id:guid}")]
        [ClaimsAuthorize("Fornecedor", "Atualizar")]
        public async Task<ActionResult> Atualizar(Guid id, ProdutoViewModel produtoViewModel)
        {
            if (id != produtoViewModel.Id)
            {
                NotificarErro("Id's informados são diferentes!");
                return CustomResponse(produtoViewModel);
            }

            var produtoAtualizacao = await _produtoRepository.ObterPorId(id);
            produtoViewModel.Imagem = produtoAtualizacao.Imagem;
            if (!ModelState.IsValid)
                return CustomResponse(ModelState);

            if(produtoViewModel.ImagemUpload != null)
            {
                var imagemNome = Guid.NewGuid() + "_" + produtoViewModel.Imagem;
                if(!UpdateArquivo(produtoViewModel.ImagemUpload, imagemNome))
                {
                    return CustomResponse(ModelState);
                }

                produtoAtualizacao.Imagem = imagemNome;
            }

            produtoAtualizacao.Nome = produtoViewModel.Nome;
            produtoAtualizacao.Descricao = produtoViewModel.Descricao;
            produtoAtualizacao.Valor = produtoViewModel.Valor;
            produtoAtualizacao.Ativo = produtoViewModel.Ativo;

            await _produtoService.Atualizar(_mapper.Map<Produto>(produtoAtualizacao));

            return CustomResponse(produtoViewModel);
                
        }

        [HttpDelete("{id:guid}")]
        [ClaimsAuthorize("Fornecedor", "Excluir")]
        public async Task<ActionResult<ProdutoViewModel>> Excluir(Guid id)
        {
            var produto = await _produtoRepository.ObterPorId(id);
            if (produto == null)
                return NotFound();

            await _produtoService.Remover(id);

            return CustomResponse(produto);
        }

        private bool UpdateArquivo(string arquivo, string imgNome)
        {
            if (string.IsNullOrEmpty(arquivo))
            {
                NotificarErro("Forneça uma imagem para este produto!");
                return false;
            }

            var imageDataByteArray = Convert.FromBase64String(arquivo);
            var caminhoArquivo = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/app/demo-webapi/src/assets", imgNome);

            if (System.IO.File.Exists(caminhoArquivo))
            {
                NotificarErro("Já Existe um arquivo com este nome!");
                return false;
            }

            System.IO.File.WriteAllBytes(caminhoArquivo, imageDataByteArray);
            return true;            
        }

        private async Task<bool> UpdateArquivoAlternativo(IFormFile arquivo, string imgPrefixo)
        {
            if (arquivo == null || arquivo.Length == 0)
            {
                NotificarErro("Forneça uma imagem para este produto!");
                return false;
            }

            var caminhoArquivo = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/app/demo-webapi/src/assets", imgPrefixo + arquivo.FileName);

            if (System.IO.File.Exists(caminhoArquivo))
            {
                NotificarErro("Já Existe um arquivo com este nome!");
                return false;
            }

            using (var stream = new FileStream(caminhoArquivo, FileMode.Create))
            {
                await arquivo.CopyToAsync(stream);
            }

            return true;
        }
    }
}
