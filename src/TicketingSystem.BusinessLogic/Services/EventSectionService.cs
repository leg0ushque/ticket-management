﻿using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using TicketingSystem.BusinessLogic.Dtos;
using TicketingSystem.BusinessLogic.Exceptions;
using TicketingSystem.BusinessLogic.Models;
using TicketingSystem.Common.Enums;
using TicketingSystem.DataAccess.Entities;
using TicketingSystem.DataAccess.Repositories;

namespace TicketingSystem.BusinessLogic.Services
{
    public class EventSectionService(IMongoRepository<EventSection> repository, IMapper mapper)
        : GenericEntityService<EventSection, EventSectionDto>(repository, mapper), IEventSectionService
    {
        // TODO CHECK IF STATE UPDATED!
        public async Task<EventSeatDto> UpdateEventSeatState(string seatId, string eventId, EventSeatState state, CancellationToken cancellationToken = default)
        {
            var eventSection = await GetSectionBySeatIdAsync(seatId, eventId, cancellationToken);
            if (eventSection.EventSeats?.Length == 0)
            {
                throw new BusinessLogicException($"Section contains no seats", null, ErrorCode.Validation);
            }

            var eventSeat = eventSection.EventSeats.FirstOrDefault(s => s.Id == seatId);
            eventSeat!.State = state;

            await _repository.UpdateAsync(eventSection.Id, es => es.EventSeats, _mapper.Map<EventSeat[]>(eventSection.EventSeats), cancellationToken);

            eventSeat.EventSectionId = eventSection.Id;
            eventSeat.EventSectionNumber = eventSection.Number;
            eventSeat.EventSectionClass = eventSection.Class;

            return eventSeat;
        }

        public async Task<List<EventSeatInfoModel>> GetSeatsInfo(string sectionId, CancellationToken cancellationToken = default)
        {
            var section = await GetByIdAsync(sectionId, cancellationToken);

            var result = section.EventSeats.Select(es => new EventSeatInfoModel
            {
                EventSectionId = sectionId,
                EventSectionNumber = section.Number,
                EventSectionClass = section.Class,
                RowNumber = es.RowNumber,
                SeatNumber = es.SeatNumber,
                Price = es.Price,
                EventSeatState = es.State,
            }).ToList();

            return result;
        }

        public async Task<List<EventSectionDto>> GetSectionsByEventIdAsync(string eventId, CancellationToken cancellationToken = default)
        {
            var result = await _repository.FilterAsync(es => es.EventId == eventId, cancellationToken);

            return _mapper.Map<List<EventSectionDto>>(result);
        }

        public async Task<EventSectionDto> GetSectionBySeatIdAsync(string seatId, string eventId = null, CancellationToken cancellationToken = default)
        {
            Expression<Func<EventSection, bool>> filteringExpression = eventId == null
                ? es => es.EventSeats.Any(s => s.Id == seatId)
                : es => es.EventId == eventId && es.EventSeats.Any(s => s.Id == seatId);

            var foundSections = await _repository.FilterAsync(filteringExpression, cancellationToken);

            var eventSection = foundSections?.FirstOrDefault();

            return eventSection == null
                ? throw new BusinessLogicException($"A section containing seat with Id {seatId} wasn't found", null, ErrorCode.NotFound)
                : _mapper.Map<EventSectionDto>(eventSection);
        }

        public Task BookSeatsOfEvent(string eventId, SectionSeatsModel[] sectionSeatsList, CancellationToken cancellationToken = default)
            => UpdateEventSeatsState(eventId, sectionSeatsList, EventSeatState.Booked, cancellationToken);

        // TODO CHECK IF WORKS!
        public async Task UpdateEventSeatsState(string eventId, SectionSeatsModel[] sectionSeatsList, EventSeatState state,
            CancellationToken cancellationToken = default)
        {
            var eventSections = await GetSectionsByEventIdAsync(eventId, cancellationToken)
                ?? throw new BusinessLogicException($"No sections were found for event with ID {eventId}", null, ErrorCode.NotFound);

            var allSectionsToUpdate = eventSections.Where(es => sectionSeatsList.Any(x => x.SectionId == es.Id)).ToList();

            foreach(var sectionSeats in sectionSeatsList)
            {
                var sectionToUpdate = allSectionsToUpdate.Find(sec => sec.Id == sectionSeats.SectionId);

                foreach (var seatIdToUpdate in sectionSeats.SeatIds)
                {
                    var seat = sectionToUpdate.EventSeats.FirstOrDefault(s => s.Id == seatIdToUpdate);

                    if (seat != null)
                    {
                        seat.State = state;
                    }
                }
            }

            foreach (var section in allSectionsToUpdate)
            {
                await _repository.UpdateAsync(section.Id, es => es.EventSeats, _mapper.Map<EventSeat[]>(section.EventSeats), cancellationToken);
            }
        }
    }
}
