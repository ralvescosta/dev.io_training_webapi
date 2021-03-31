using AutoMapper;
using DevIO.Api.Extensions;
using DevIO.Api.ViewModels;
using DevIO.Business.Intefaces;
using DevIO.Business.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevIO.Api.Controllers
{
    [Route("api/fornecedores")]
    [Authorize]
    public class FornecedoresController : MainController
    {
        private readonly IFornecedorRepository _fornecedorRepository;
        private readonly IFornecedorService _fornecedorService;
        private readonly IEnderecoRepository _enderecoRepository;
        private readonly IMapper _mapper;
        private readonly IUser _user;

        public FornecedoresController(
            IFornecedorRepository fornecedorRepository,
            IMapper mapper,
            IFornecedorService fornecedorService,
            INotificador notificador, 
            IEnderecoRepository enderecoRepository,
            IUser user) : base(notificador)
        {
            _fornecedorRepository = fornecedorRepository;
            _mapper = mapper;
            _fornecedorService = fornecedorService;
            _enderecoRepository = enderecoRepository;
            _user = user;
        }

        [HttpPost]
        [ClaimsAuthorize("Fornecedor", "Adicionar")]
        public async Task<ActionResult<FornecedorViewModel>> Adicionar(FornecedorViewModel fornecedorViewModel)
        {
            if (!ModelState.IsValid) 
                return CustomResponse(ModelState);

            await _fornecedorService.Adicionar(_mapper.Map<Fornecedor>(fornecedorViewModel));

            return CustomResponse(fornecedorViewModel);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<FornecedorViewModel>>> ObterTodos()
        {
            var fornecedor = _mapper.Map<IEnumerable<FornecedorViewModel>>(await _fornecedorRepository.ObterTodos());
            return CustomResponse(fornecedor);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<FornecedorViewModel>> ObterPorId(Guid id)
        {
            var fornecedor = _mapper.Map<FornecedorViewModel>(await _fornecedorRepository.ObterFornecedorProdutosEndereco(id));

            if (fornecedor == null)
                return NotFound();

            return CustomResponse(fornecedor);
        }

        [HttpGet("obter-endereco/{id:guid}")]
        public async Task<ActionResult<FornecedorViewModel>> ObterEnderecoPorId(Guid id)
        {
            var fornecedor = _mapper.Map<FornecedorViewModel>(await _enderecoRepository.ObterPorId(id));

            if (fornecedor == null)
                return NotFound();

            return CustomResponse(fornecedor);
        }

        [HttpPut("{id:guid}")]
        [ClaimsAuthorize("Fornecedor", "Atualizar")]
        public async Task<ActionResult<FornecedorViewModel>> Atualizar(Guid id, FornecedorViewModel fornecedorViewModel)
        {
            if (id != fornecedorViewModel.Id) 
                return BadRequest();

            if (!ModelState.IsValid) 
                return CustomResponse(ModelState);

            await _fornecedorService.Atualizar(_mapper.Map<Fornecedor>(fornecedorViewModel));

            return CustomResponse(fornecedorViewModel);
        }

        [HttpPut("atualiza-endereco/{id:guid}")]
        [ClaimsAuthorize("Fornecedor", "Atualizar")]
        public async Task<ActionResult<FornecedorViewModel>> AtualizarEndereco(Guid id, EnderecoViewModel enderecoViewModel)
        {
            if (id != enderecoViewModel.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return CustomResponse(ModelState);

            await _fornecedorService.AtualizarEndereco(_mapper.Map<Endereco>(enderecoViewModel));

            return CustomResponse(enderecoViewModel);
        }

        [HttpDelete("{id:guid}")]
        [ClaimsAuthorize("Fornecedor", "Excluir")]
        public async Task<ActionResult<FornecedorViewModel>> Excluir(Guid id)
        {
            var fornecedor = _mapper.Map<FornecedorViewModel>(await _fornecedorRepository.ObterFornecedorProdutosEndereco(id));

            if (fornecedor == null) 
                return NotFound();

            await _fornecedorService.Remover(id);

            return CustomResponse(fornecedor);
        }
    }
}
