// src/pages/TimeSlotsPage.jsx
import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import axios from 'axios';

const TimeSlotsPage = ({ bookingData, updateBookingData }) => {
    const [timeSlots, setTimeSlots] = useState([]);
    const [selectedDate, setSelectedDate] = useState(null);
    const [selectedSlot, setSelectedSlot] = useState(bookingData.selectedTimeSlot || null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');
    const navigate = useNavigate();

    // Generate dates for the next 14 days
    const generateDates = () => {
        const dates = [];
        const today = new Date();
        for (let i = 0; i < 14; i++) {
            const date = new Date(today);
            date.setDate(today.getDate() + i);
            dates.push(date);
        }
        return dates;
    };

    const [availableDates] = useState(generateDates());

    // Mock time slots - in real app, fetch based on selected date
    const mockTimeSlots = [
        { id: 1, date: '2024-01-15', startTime: '08:00', endTime: '10:00', available: true },
        { id: 2, date: '2024-01-15', startTime: '10:00', endTime: '12:00', available: true },
        { id: 3, date: '2024-01-15', startTime: '12:00', endTime: '14:00', available: true },
        { id: 4, date: '2024-01-15', startTime: '14:00', endTime: '16:00', available: true },
        { id: 5, date: '2024-01-15', startTime: '16:00', endTime: '18:00', available: false },
        { id: 6, date: '2024-01-16', startTime: '08:00', endTime: '10:00', available: true },
        { id: 7, date: '2024-01-16', startTime: '10:00', endTime: '12:00', available: true },
        { id: 8, date: '2024-01-16', startTime: '12:00', endTime: '14:00', available: false },
        { id: 9, date: '2024-01-16', startTime: '14:00', endTime: '16:00', available: true },
        { id: 10, date: '2024-01-16', startTime: '16:00', endTime: '18:00', available: true },
    ];

    useEffect(() => {
        setTimeout(() => {
            setTimeSlots(mockTimeSlots);
            setLoading(false);
            // Auto-select first available date
            if (availableDates.length > 0) {
                setSelectedDate(availableDates[0]);
            }
        }, 1000);
    }, []);

    const formatDate = (date) => {
        return date.toLocaleDateString('en-GB', {
            weekday: 'short',
            day: 'numeric',
            month: 'short'
        });
    };

    const formatDateFull = (date) => {
        return date.toLocaleDateString('en-GB', {
            weekday: 'long',
            day: 'numeric',
            month: 'long',
            year: 'numeric'
        });
    };

    const formatTime = (timeString) => {
        return new Date(`1970-01-01T${timeString}`).toLocaleTimeString('en-GB', {
            hour: '2-digit',
            minute: '2-digit',
            hour12: true
        });
    };

    const getTimeSlotsForDate = (date) => {
        if (!date) return [];
        const dateStr = date.toISOString().split('T')[0];
        return timeSlots.filter(slot => slot.date === dateStr);
    };

    const handleContinue = () => {
        if (!selectedSlot) {
            setError('Please select a date and time slot');
            window.scrollTo({ top: 0, behavior: 'smooth' });
            return;
        }

        updateBookingData({ selectedTimeSlot: selectedSlot });
        navigate('/payment');
    };

    const calculateTotalDuration = () => {
        return bookingData.selectedServicesData?.reduce((total, service) =>
            total + service.duration, 0) || 60;
    };

    const isToday = (date) => {
        const today = new Date();
        return date.toDateString() === today.toDateString();
    };

    const isTomorrow = (date) => {
        const tomorrow = new Date();
        tomorrow.setDate(tomorrow.getDate() + 1);
        return date.toDateString() === tomorrow.toDateString();
    };

    if (loading) {
        return (
            <div className="flex justify-center items-center min-h-[60vh]">
                <div className="text-center">
                    <div
                        className="animate-spin rounded-full h-16 w-16 border-4 border-t-transparent mx-auto"
                        style={{ borderColor: '#46C6CE', borderTopColor: 'transparent' }}
                    ></div>
                    <p className="mt-6 text-gray-600 font-medium text-lg">Loading available slots...</p>
                </div>
            </div>
        );
    }

    const slotsForSelectedDate = getTimeSlotsForDate(selectedDate);

    return (
        <div className="min-h-screen bg-gradient-to-br from-gray-50 via-white to-blue-50/30 py-8 px-4">
            <div className="max-w-6xl mx-auto">
                {/* Header */}
                <div className="text-center mb-10">
                    <div className="inline-block px-4 py-2 bg-gradient-to-r from-[#194376]/10 to-[#46C6CE]/10 rounded-full mb-4">
                        <span className="text-[#194376] font-bold text-sm">Step 3 of 4</span>
                    </div>
                    <h1 className="text-4xl font-bold text-gray-800 mb-3">
                        Choose Your <span className="text-[#194376]">Date & Time</span>
                    </h1>
                    <p className="text-gray-600 text-lg">
                        Select a convenient time slot for your cleaning service
                    </p>
                </div>

                {/* Booking Summary */}
                <div
                    className="bg-white rounded-2xl p-6 mb-10 border-2 shadow-lg"
                    style={{ borderColor: '#46C6CE' }}
                >
                    <div className="flex items-center mb-4">
                        <div
                            className="w-10 h-10 rounded-lg flex items-center justify-center mr-3"
                            style={{ background: '#194376' }}
                        >
                            <svg className="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2" />
                            </svg>
                        </div>
                        <h3 className="text-xl font-bold text-gray-800">Booking Summary</h3>
                    </div>
                    <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
                        <div className="bg-gradient-to-br from-gray-50 to-gray-100 p-4 rounded-xl">
                            <p className="text-xs text-gray-500 mb-1 font-semibold uppercase tracking-wide">Services</p>
                            <p className="font-bold text-gray-800 text-sm leading-tight">
                                {bookingData.selectedServicesData?.map(s => s.name).join(', ') || 'No services'}
                            </p>
                        </div>
                        <div className="bg-gradient-to-br from-gray-50 to-gray-100 p-4 rounded-xl">
                            <p className="text-xs text-gray-500 mb-1 font-semibold uppercase tracking-wide">Duration</p>
                            <p className="font-bold text-2xl text-[#194376]">{calculateTotalDuration()} mins</p>
                        </div>
                        <div className="bg-gradient-to-br from-gray-50 to-gray-100 p-4 rounded-xl">
                            <p className="text-xs text-gray-500 mb-1 font-semibold uppercase tracking-wide">Total</p>
                            <p className="font-bold text-2xl text-[#194376]">£{bookingData.totalAmount?.toFixed(2) || '0.00'}</p>
                        </div>
                    </div>
                </div>

                {/* Error Message */}
                {error && (
                    <div className="bg-red-50 border-l-4 border-red-500 rounded-lg p-4 mb-8 animate-in fade-in slide-in-from-top duration-300">
                        <div className="flex items-center">
                            <svg className="h-5 w-5 text-red-500 mr-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                            </svg>
                            <p className="text-red-700 font-semibold">{error}</p>
                        </div>
                    </div>
                )}

                {/* Date Selection Calendar */}
                <div className="bg-white rounded-2xl p-6 mb-8 shadow-lg">
                    <h2 className="text-2xl font-bold text-gray-800 mb-6 flex items-center">
                        <svg className="w-6 h-6 mr-2 text-[#194376]" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z" />
                        </svg>
                        Select a Date
                    </h2>

                    <div className="grid grid-cols-3 sm:grid-cols-4 md:grid-cols-5 lg:grid-cols-7 gap-3">
                        {availableDates.map((date, index) => {
                            const isSelected = selectedDate && date.toDateString() === selectedDate.toDateString();
                            const todayLabel = isToday(date);
                            const tomorrowLabel = isTomorrow(date);

                            return (
                                <div
                                    key={index}
                                    onClick={() => {
                                        setSelectedDate(date);
                                        setSelectedSlot(null);
                                        setError('');
                                    }}
                                    className={`
                                        relative p-4 rounded-xl cursor-pointer transition-all duration-300 text-center
                                        ${isSelected
                                        ? 'shadow-xl transform scale-105'
                                        : 'hover:shadow-md hover:scale-102'
                                    }
                                    `}
                                    style={isSelected ? {
                                        background: 'linear-gradient(135deg, #194376 0%, #46C6CE 100%)',
                                        color: 'white'
                                    } : {
                                        background: 'linear-gradient(to bottom, #f9fafb, #f3f4f6)',
                                        border: '2px solid #e5e7eb'
                                    }}
                                >
                                    {(todayLabel || tomorrowLabel) && (
                                        <div
                                            className="absolute -top-2 left-1/2 transform -translate-x-1/2 px-2 py-0.5 rounded-full text-xs font-bold"
                                            style={{
                                                backgroundColor: isSelected ? 'white' : '#46C6CE',
                                                color: isSelected ? '#194376' : 'white'
                                            }}
                                        >
                                            {todayLabel ? 'Today' : 'Tomorrow'}
                                        </div>
                                    )}
                                    <div className={`text-xs font-semibold mb-1 ${isSelected ? 'text-white/90' : 'text-gray-500'}`}>
                                        {date.toLocaleDateString('en-GB', { weekday: 'short' })}
                                    </div>
                                    <div className={`text-2xl font-bold ${isSelected ? 'text-white' : 'text-gray-800'}`}>
                                        {date.getDate()}
                                    </div>
                                    <div className={`text-xs font-medium ${isSelected ? 'text-white/90' : 'text-gray-600'}`}>
                                        {date.toLocaleDateString('en-GB', { month: 'short' })}
                                    </div>
                                </div>
                            );
                        })}
                    </div>
                </div>

                {/* Time Slots */}
                {selectedDate && (
                    <div className="bg-white rounded-2xl p-6 shadow-lg mb-8">
                        <h2 className="text-2xl font-bold text-gray-800 mb-4 flex items-center">
                            <svg className="w-6 h-6 mr-2 text-[#194376]" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z" />
                            </svg>
                            Available Times for {formatDateFull(selectedDate)}
                        </h2>

                        {slotsForSelectedDate.length === 0 ? (
                            <div className="text-center py-12 bg-yellow-50 rounded-xl border-2 border-yellow-200">
                                <svg className="w-16 h-16 text-yellow-500 mx-auto mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                                </svg>
                                <p className="text-yellow-800 text-lg font-semibold mb-2">No slots available</p>
                                <p className="text-yellow-700">Please select another date</p>
                            </div>
                        ) : (
                            <div className="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-5 gap-3">
                                {slotsForSelectedDate.map(slot => {
                                    const isSelected = selectedSlot?.id === slot.id;

                                    return (
                                        <button
                                            key={slot.id}
                                            onClick={() => slot.available && setSelectedSlot(slot)}
                                            disabled={!slot.available}
                                            className={`
                                                p-4 rounded-xl font-bold transition-all duration-300 relative
                                                ${!slot.available
                                                ? 'bg-gray-100 text-gray-400 cursor-not-allowed border-2 border-gray-200'
                                                : isSelected
                                                    ? 'text-white shadow-xl transform scale-105'
                                                    : 'bg-white border-2 border-gray-200 text-gray-700 hover:border-[#46C6CE] hover:shadow-lg hover:scale-105'
                                            }
                                            `}
                                            style={isSelected && slot.available ? {
                                                background: 'linear-gradient(135deg, #194376 0%, #46C6CE 100%)'
                                            } : {}}
                                        >
                                            <div className="text-sm mb-1">
                                                {formatTime(slot.startTime)}
                                            </div>
                                            <div className="text-xs opacity-75">
                                                {formatTime(slot.endTime)}
                                            </div>
                                            {!slot.available && (
                                                <div className="absolute top-1 right-1">
                                                    <svg className="w-4 h-4 text-red-400" fill="currentColor" viewBox="0 0 20 20">
                                                        <path fillRule="evenodd" d="M13.477 14.89A6 6 0 015.11 6.524l8.367 8.368zm1.414-1.414L6.524 5.11a6 6 0 018.367 8.367zM18 10a8 8 0 11-16 0 8 8 0 0116 0z" clipRule="evenodd" />
                                                    </svg>
                                                </div>
                                            )}
                                            {isSelected && (
                                                <div className="absolute -top-2 -right-2 w-6 h-6 bg-white rounded-full flex items-center justify-center">
                                                    <svg className="w-4 h-4 text-[#194376]" fill="currentColor" viewBox="0 0 20 20">
                                                        <path fillRule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clipRule="evenodd" />
                                                    </svg>
                                                </div>
                                            )}
                                        </button>
                                    );
                                })}
                            </div>
                        )}
                    </div>
                )}

                {/* Selected Slot Confirmation */}
                {selectedSlot && (
                    <div
                        className="bg-white rounded-2xl p-6 mb-8 border-2 shadow-lg animate-in fade-in slide-in-from-bottom duration-500"
                        style={{ borderColor: '#46C6CE' }}
                    >
                        <div className="flex items-center justify-between">
                            <div className="flex items-center gap-4">
                                <div
                                    className="w-12 h-12 rounded-xl flex items-center justify-center"
                                    style={{ background: 'linear-gradient(135deg, #194376 0%, #46C6CE 100%)' }}
                                >
                                    <svg className="w-6 h-6 text-white" fill="currentColor" viewBox="0 0 20 20">
                                        <path fillRule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clipRule="evenodd" />
                                    </svg>
                                </div>
                                <div>
                                    <p className="text-sm text-gray-500 font-semibold">Your Appointment</p>
                                    <p className="text-xl font-bold text-[#194376]">
                                        {formatDateFull(selectedDate)} • {formatTime(selectedSlot.startTime)} - {formatTime(selectedSlot.endTime)}
                                    </p>
                                </div>
                            </div>
                            <button
                                onClick={() => setSelectedSlot(null)}
                                className="text-gray-400 hover:text-gray-600 transition"
                            >
                                <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M6 18L18 6M6 6l12 12" />
                                </svg>
                            </button>
                        </div>
                    </div>
                )}

                {/* Action Buttons */}
                <div className="flex flex-col sm:flex-row justify-between items-center gap-4 pt-6 border-t-2 border-gray-200">
                    <button
                        onClick={() => navigate('/services')}
                        className="px-8 py-4 border-2 border-gray-300 text-gray-700 font-bold rounded-xl hover:bg-gray-50 hover:border-gray-400 transition-all w-full sm:w-auto flex items-center justify-center gap-2"
                    >
                        <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M15 19l-7-7 7-7" />
                        </svg>
                        Back to Services
                    </button>

                    <button
                        onClick={handleContinue}
                        disabled={!selectedSlot}
                        className={`
                            px-8 py-4 font-bold rounded-xl transition-all w-full sm:w-auto flex items-center justify-center gap-2
                            ${!selectedSlot
                            ? 'bg-gray-300 text-gray-500 cursor-not-allowed'
                            : 'text-white hover:shadow-2xl transform hover:scale-105 active:scale-95'
                        }
                        `}
                        style={selectedSlot ? {
                            background: 'linear-gradient(135deg, #194376 0%, #2a5a94 50%, #46C6CE 100%)'
                        } : {}}
                    >
                        Continue to Payment
                        <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2.5" d="M13 7l5 5m0 0l-5 5m5-5H6" />
                        </svg>
                    </button>
                </div>
            </div>
        </div>
    );
};

export default TimeSlotsPage;