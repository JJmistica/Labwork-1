using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using SampleWebApiAspNetCore.Dtos;
using SampleWebApiAspNetCore.Entities;
using SampleWebApiAspNetCore.Helpers;
using SampleWebApiAspNetCore.Services;
using SampleWebApiAspNetCore.Models;
using SampleWebApiAspNetCore.Repositories;
using System.Text.Json;

namespace SampleWebApiAspNetCore.Controllers.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class SnackController : ControllerBase
    {
        private readonly SnackRepository _snackRepository;
        private readonly IMapper _mapper;
        private readonly ILinkService<SnackController> _linkService;

        public SnackController(
            SnackRepository foodRepository,
            IMapper mapper,
            ILinkService<SnackController> linkService)
        {
            _snackRepository = foodRepository;
            _mapper = mapper;
            _linkService = linkService;
        }

        [HttpGet(Name = nameof(GetAllFoods))]
        public ActionResult GetAllFoods(ApiVersion version, [FromQuery] QueryParameters queryParameters)
        {
            List<SnackEntity> foodItems = _snackRepository.GetAll(queryParameters).ToList();

            var allItemCount = _snackRepository.Count();

            var paginationMetadata = new
            {
                totalCount = allItemCount,
                pageSize = queryParameters.PageCount,
                currentPage = queryParameters.Page,
                totalPages = queryParameters.GetTotalPages(allItemCount)
            };

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

            var links = _linkService.CreateLinksForCollection(queryParameters, allItemCount, version);
            var toReturn = foodItems.Select(x => _linkService.ExpandSingleSnackItem(x, x.Id, version));

            return Ok(new
            {
                value = toReturn,
                links = links
            });
        }

        [HttpGet]
        [Route("{id:int}", Name = nameof(GetSingleFood))]
        public ActionResult GetSingleFood(ApiVersion version, int id)
        {
            SnackEntity foodItem = _snackRepository.GetSingle(id);

            if (foodItem == null)
            {
                return NotFound();
            }

            SnackDto item = _mapper.Map<SnackDto>(foodItem);

            return Ok(_linkService.ExpandSingleSnackItem(item, item.Id, version));
        }

        [HttpPost(Name = nameof(AddSnack))]
        public ActionResult<SnackDto> AddSnack(ApiVersion version, [FromBody] SnackCreateDto snackCreateDto)
        {
            if (snackCreateDto == null)
            {
                return BadRequest();
            }

            SnackEntity toAdd = _mapper.Map<SnackEntity>(snackCreateDto);

            _snackRepository.Add(toAdd);

            if (!_snackRepository.Save())
            {
                throw new Exception("Creating a fooditem failed on save.");
            }

            SnackEntity newSnackItem = _snackRepository.GetSingle(toAdd.Id);
            SnackDto snackDto = _mapper.Map<SnackDto>(newSnackItem);

            return CreatedAtRoute(nameof(GetSingleFood),
                new { version = version.ToString(), id = newSnackItem.Id },
                _linkService.ExpandSingleSnackItem(snackDto, snackDto.Id, version));
        }

        [HttpPatch("{id:int}", Name = nameof(PartiallyUpdateFood))]
        public ActionResult<SnackDto> PartiallyUpdateFood(ApiVersion version, int id, [FromBody] JsonPatchDocument<SnackUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            SnackEntity existingEntity = _snackRepository.GetSingle(id);

            if (existingEntity == null)
            {
                return NotFound();
            }

            SnackUpdateDto snackUpdateDto = _mapper.Map<SnackUpdateDto>(existingEntity);
            patchDoc.ApplyTo(snackUpdateDto);

            TryValidateModel(snackUpdateDto);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(snackUpdateDto, existingEntity);
            SnackEntity updated = _snackRepository.Update(id, existingEntity);

            if (!_snackRepository.Save())
            {
                throw new Exception("Updating a fooditem failed on save.");
            }

            SnackDto snackDto = _mapper.Map<SnackDto>(updated);

            return Ok(_linkService.ExpandSingleSnackItem(snackDto, snackDto.Id, version));
        }

        [HttpDelete]
        [Route("{id:int}", Name = nameof(RemoveSnack))]
        public ActionResult RemoveSnack(int id)
        {
            SnackEntity snackItem = _snackRepository.GetSingle(id);

            if (snackItem == null)
            {
                return NotFound();
            }

            _snackRepository.Delete(id);

            if (!_snackRepository.Save())
            {
                throw new Exception("Deleting a snackitem failed on save.");
            }

            return NoContent();
        }

        [HttpPut]
        [Route("{id:int}", Name = nameof(UpdateSnack))]
        public ActionResult<SnackDto> UpdateSnack(ApiVersion version, int id, [FromBody] SnackUpdateDto snackUpdateDto)
        {
            if (snackUpdateDto == null)
            {
                return BadRequest();
            }

            var existingSnackItem = _snackRepository.GetSingle(id);

            if (existingSnackItem == null)
            {
                return NotFound();
            }

            _mapper.Map(snackUpdateDto, existingSnackItem);

            _snackRepository.Update(id, existingSnackItem);

            if (!_snackRepository.Save())
            {
                throw new Exception("Updating a snackitem failed on save.");
            }

            SnackDto snackDto = _mapper.Map<SnackDto>(existingSnackItem);

            return Ok(_linkService.ExpandSingleSnackItem(snackDto, snackDto.Id, version));
        }

        [HttpGet("GetRandomMeal", Name = nameof(GetRandomMeal))]
        public ActionResult GetRandomMeal()
        {
            ICollection<SnackEntity> snackItems = _snackRepository.GetRandomMeal();

            IEnumerable<SnackDto> dtos = snackItems.Select(x => _mapper.Map<SnackDto>(x));

            var links = new List<LinkDto>();

            // self 
            links.Add(new LinkDto(Url.Link(nameof(GetRandomMeal), null), "self", "GET"));

            return Ok(new
            {
                value = dtos,
                links = links
            });
        }
    }
}
