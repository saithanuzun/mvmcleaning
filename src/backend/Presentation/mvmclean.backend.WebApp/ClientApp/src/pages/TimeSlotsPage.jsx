// src/pages/TimeSlotsPage.jsx
import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import api from '../services/api';

const TimeSlotsPage = ({ bookingData, updateBookingData }) => {
    const [timeSlots, setTimeSlots] = useState([]);
    const [selectedDate, setSelectedDate] = useState(bookingData.selectedDate || null);
    const [selectedSlot, setSelectedSlot] = useState(bookingData.selectedTimeSlot || null);
    const [loading, setLoading] = useState(false);
    const [loadingSlots, setLoadingSlots] = useState(false);
    const [error, setError] = useState('');
    const [currentMonth, setCurrentMonth] = useState(0); // 0 = current month, 1 = next month, etc.
    const navigate = useNavigate();

    // Helper function to get month name
    const getMonthName = (monthIndex) => {
        const months = [
            'January', 'February', 'March', 'April', 'May', 'June',
            'July', 'August', 'September', 'October', 'November', 'December'
        ];
        return months[monthIndex % 12];
    };

    // Helper function to get calendar days for a specific month offset (0 = current month, 1 = next month, etc.)
    const getCalendarDays = (monthOffset) => {
        const today = new Date();
        const year = today.getFullYear();
        const month = today.getMonth() + monthOffset;

        // First day of the month
        const firstDay = new Date(year, month, 1);
        // Last day of the month
        const lastDay = new Date(year, month + 1, 0);

        // Day of week for first day (0 = Sunday, 6 = Saturday)
        const firstDayOfWeek = firstDay.getDay();

        const days = [];

        // Add empty cells for days before the first day of the month
        for (let i = 0; i < firstDayOfWeek; i++) {
            days.push(null);
        }

        // Add days of the month
        for (let day = 1; day <= lastDay.getDate(); day++) {
            const date = new Date(year, month, day);
            days.push(date);
        }

        return days;
    };

    // Generate dates for the next 2 months
    const generateDates = () => {
        const dates = [];
        const today = new Date();
        // Get dates for next 60 days (approx 2 months)
        for (let i = 0; i < 60; i++) {
            const date = new Date(today);
            date.setDate(today.getDate() + i);
            dates.push(date);
        }
        return dates;
    };

    const [availableDates] = useState(generateDates());

    // Redirect if no postcode or services selected
    useEffect(() => {
        if (!bookingData.postcode || !bookingData.selectedServices) {
            navigate('/services');
        }
    }, [bookingData.postcode, bookingData.selectedServices, navigate]);

    // Fetch time slots on mount if date was previously selected (for back navigation)
    useEffect(() => {
        if (selectedDate && bookingData.postcode && bookingData.totalDuration) {
            fetchTimeSlots(selectedDate);
        }
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, []);

    // Fetch available time slots when date is selected
    const fetchTimeSlots = async (date) => {
        if (!date || !bookingData.postcode || !bookingData.totalDuration) return;

        try {
            setLoadingSlots(true);
            setError('');

            const dateStr = date.toLocaleDateString('en-CA');
            const contractorIds = bookingData.contractors || [];

            const response = await api.availability.getSlots(
                bookingData.postcode,
                dateStr,
                bookingData.totalDuration,
                contractorIds
            );

            if (response.success && response.data) {
                setTimeSlots(response.data.availableSlots || []);
            } else {
                setError(response.message || 'Failed to load available time slots');
                setTimeSlots([]);
            }
        } catch (err) {
            console.error('Error fetching time slots:', err);
            setError('Failed to load available time slots. Please try again.');
            setTimeSlots([]);
        } finally {
            setLoadingSlots(false);
        }
    };

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

    const getTimeSlotsForDate = () => {
        // Backend returns slots for the selected date only
        return timeSlots;
    };

    const handleContinue = () => {
        if (!selectedSlot) {
            setError('Please select a date and time slot');
            window.scrollTo({ top: 0, behavior: 'smooth' });
            return;
        }

        updateBookingData({
            selectedTimeSlot: selectedSlot,
            selectedDate: selectedDate,
            contractorId: selectedSlot.contractorId,
            contractorName: selectedSlot.contractorName
        });
        navigate('/payment');
    };

    const calculateTotalDuration = () => {
        return bookingData.totalDuration || 60;
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

    const goToPreviousMonth = () => {
        if (currentMonth > 0) {
            setCurrentMonth(currentMonth - 1);
        }
    };

    const goToNextMonth = () => {
        if (currentMonth < 3) { // Show up to 4 months ahead (0-3)
            setCurrentMonth(currentMonth + 1);
        }
    };

    const getMonthYear = () => {
        const today = new Date();
        const year = today.getFullYear();
        const month = today.getMonth() + currentMonth;
        return { month, year };
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
    const { month, year } = getMonthYear();

    return (
        <div className="min-h-screen bg-gray-50 py-8 px-4">
            <div className="max-w-6xl mx-auto">
                {/* Header */}
                <div className="text-center mb-10">
                    <div className="inline-block px-4 py-2 bg-blue-50 rounded-full mb-4">
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
                        <div className="bg-gray-50 p-4 rounded-xl">
                            <p className="text-xs text-gray-500 mb-1 font-semibold uppercase tracking-wide">Services</p>
                            <p className="font-bold text-gray-800 text-sm leading-tight">
                                {bookingData.basket?.items?.map(item => item.serviceName).join(', ') || 'No services'}
                            </p>
                        </div>
                        <div className="bg-gray-50 p-4 rounded-xl">
                            <p className="text-xs text-gray-500 mb-1 font-semibold uppercase tracking-wide">Duration</p>
                            <p className="font-bold text-2xl text-[#194376]">{calculateTotalDuration()} mins</p>
                        </div>
                        <div className="bg-gray-50 p-4 rounded-xl">
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

                {/* Date Selection Calendar - Single Month with Navigation */}
                <div className="bg-white rounded-2xl p-6 mb-8 shadow-lg">
                    <div className="flex items-center justify-between mb-6">
                        <h2 className="text-2xl font-bold text-gray-800 flex items-center">
                            <svg className="w-6 h-6 mr-2 text-[#194376]" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z" />
                            </svg>
                            Select a Date
                        </h2>

                        <div className="flex items-center space-x-2">
                            <button
                                onClick={goToPreviousMonth}
                                disabled={currentMonth === 0}
                                className={`p-2 rounded-lg transition-all ${currentMonth === 0 ? 'opacity-30 cursor-not-allowed' : 'hover:bg-gray-100 hover:shadow-sm'}`}
                            >
                                <svg className="w-5 h-5 text-[#194376]" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M15 19l-7-7 7-7" />
                                </svg>
                            </button>

                            <div className="text-lg font-semibold text-gray-700 min-w-[180px] text-center">
                                {getMonthName(month % 12)} {year}
                            </div>

                            <button
                                onClick={goToNextMonth}
                                disabled={currentMonth === 3}
                                className={`p-2 rounded-lg transition-all ${currentMonth === 3 ? 'opacity-30 cursor-not-allowed' : 'hover:bg-gray-100 hover:shadow-sm'}`}
                            >
                                <svg className="w-5 h-5 text-[#194376]" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M9 5l7 7-7 7" />
                                </svg>
                            </button>
                        </div>
                    </div>

                    {/* Month Display */}
                    <div>
                        <div className="grid grid-cols-7 gap-1.5 mb-2">
                            {/* Day headers */}
                            {['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'].map(day => (
                                <div key={day} className="text-center text-xs font-semibold text-gray-500 py-2">
                                    {day}
                                </div>
                            ))}
                        </div>

                        <div className="grid grid-cols-7 gap-1.5">
                            {/* Calendar days for current month */}
                            {getCalendarDays(currentMonth).map((date, index) => {
                                const isSelected = selectedDate && date && date.toDateString() === selectedDate.toDateString();
                                const todayLabel = date && isToday(date);
                                const tomorrowLabel = date && isTomorrow(date);
                                const isCurrentMonth = date && date.getMonth() === month % 12;

                                return (
                                    <div
                                        key={`month-${index}`}
                                        onClick={() => {
                                            if (date && isCurrentMonth) {
                                                setSelectedDate(date);
                                                setSelectedSlot(null);
                                                setError('');
                                                fetchTimeSlots(date);
                                            }
                                        }}
                                        className={`
                                            relative p-2.5 rounded-lg text-center min-h-[70px]
                                            ${date && isCurrentMonth ? 'cursor-pointer hover:bg-gray-50 hover:shadow-sm' : ''}
                                            ${isSelected ? 'bg-blue-50 border border-[#194376] shadow-sm' : 'border border-gray-100'}
                                            ${!date || !isCurrentMonth ? 'opacity-40' : ''}
                                        `}
                                    >
                                        {(todayLabel || tomorrowLabel) && date && isCurrentMonth && (
                                            <div className="absolute -top-1.5 left-1/2 transform -translate-x-1/2 px-1.5 py-0.5 rounded-full text-[10px] font-bold bg-[#46C6CE] text-white">
                                                {todayLabel ? 'Today' : 'Tomorrow'}
                                            </div>
                                        )}
                                        {date && (
                                            <>
                                                <div className={`text-[10px] font-medium mb-0.5 ${isSelected ? 'text-[#194376]' : 'text-gray-500'}`}>
                                                    {date.getDate() === 1 ? date.toLocaleDateString('en-GB', { month: 'short' }) : ''}
                                                </div>
                                                <div className={`text-base font-bold ${isSelected ? 'text-[#194376]' : isCurrentMonth ? 'text-gray-800' : 'text-gray-400'}`}>
                                                    {date.getDate()}
                                                </div>
                                                <div className={`text-[10px] ${isSelected ? 'text-[#194376]' : isCurrentMonth ? 'text-gray-600' : 'text-gray-400'}`}>
                                                    {date.toLocaleDateString('en-GB', { weekday: 'short' }).substring(0, 1)}
                                                </div>
                                            </>
                                        )}
                                    </div>
                                );
                            })}
                        </div>
                    </div>

                    {/* Month Navigation Indicator */}
                    <div className="flex justify-center items-center mt-6 space-x-2">
                        {[0, 1, 2, 3].map((monthIndex) => (
                            <button
                                key={monthIndex}
                                onClick={() => setCurrentMonth(monthIndex)}
                                className={`h-2 rounded-full transition-all ${currentMonth === monthIndex
                                    ? 'w-8 bg-[#194376]'
                                    : 'w-2 bg-gray-300 hover:bg-gray-400'
                                    }`}
                            />
                        ))}
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

                        {loadingSlots ? (
                            <div className="flex justify-center items-center py-12">
                                <div className="text-center">
                                    <div
                                        className="animate-spin rounded-full h-12 w-12 border-4 border-t-transparent mx-auto"
                                        style={{ borderColor: '#46C6CE', borderTopColor: 'transparent' }}
                                    ></div>
                                    <p className="mt-4 text-gray-600 font-medium">Loading available slots...</p>
                                </div>
                            </div>
                        ) : slotsForSelectedDate.length === 0 ? (
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
                                    const isSelected = selectedSlot?.slotId === slot.slotId;

                                    return (
                                        <button
                                            key={slot.slotId}
                                            onClick={() => slot.isAvailable && setSelectedSlot(slot)}
                                            disabled={!slot.isAvailable}
                                            className={`
                                                p-4 rounded-xl font-bold transition-all duration-300 relative
                                                ${!slot.isAvailable
                                                    ? 'bg-gray-100 text-gray-400 cursor-not-allowed border-2 border-gray-200'
                                                    : isSelected
                                                        ? 'bg-[#194376] text-white shadow-xl transform scale-105'
                                                        : 'bg-white border-2 border-gray-200 text-gray-700 hover:border-[#46C6CE] hover:shadow-lg hover:scale-105'
                                                }
                                            `}
                                        >
                                            <div className="text-sm mb-1">
                                                {formatTime(slot.startTime)}
                                            </div>
                                            <div className="text-xs opacity-75 mb-1">
                                                {formatTime(slot.endTime)}
                                            </div>
                                            {slot.contractorName && (
                                                <div className="text-[10px] opacity-70 mt-1 truncate">
                                                    {slot.contractorName}
                                                </div>
                                            )}
                                            {!slot.isAvailable && (
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
                                    className="w-12 h-12 rounded-xl flex items-center justify-center bg-[#194376]"
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
                                    {selectedSlot.contractorName && (
                                        <p className="text-sm text-gray-600 mt-1">
                                            Contractor: <span className="font-semibold">{selectedSlot.contractorName}</span>
                                        </p>
                                    )}
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
                                : 'bg-[#46C6CE] text-white hover:shadow-2xl transform hover:scale-105 active:scale-95'
                            }
                        `}
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