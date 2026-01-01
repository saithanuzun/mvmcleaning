// src/App.js
import React, { useState } from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import PostcodePage from './pages/PostcodePage';
import ServicesPage from './pages/ServicesPage';
import TimeSlotsPage from './pages/TimeSlotsPage';
import PaymentPage from './pages/BookingPage';
import ProgressBar from './components/ProgressBar';

function App() {
    const [bookingData, setBookingData] = useState({
        postcode: '',
        phone: '',
        selectedServices: [],
        selectedTimeSlot: null,
        customerDetails: {},
        totalAmount: 0
    });

    const updateBookingData = (newData) => {
        setBookingData(prev => ({ ...prev, ...newData }));
    };

    return (
        <Router>
            <div className="min-h-screen bg-gray-50">
                <ProgressBar />
                <div className="container mx-auto px-4 py-8">
                    <Routes>
                        <Route path="/" element={<Navigate to="/postcode" />} />
                        <Route
                            path="/postcode"
                            element={<PostcodePage bookingData={bookingData} updateBookingData={updateBookingData} />}
                        />
                        <Route
                            path="/services"
                            element={<ServicesPage bookingData={bookingData} updateBookingData={updateBookingData} />}
                        />
                        <Route
                            path="/time-slots"
                            element={<TimeSlotsPage bookingData={bookingData} updateBookingData={updateBookingData} />}
                        />
                        <Route
                            path="/payment"
                            element={<PaymentPage bookingData={bookingData} updateBookingData={updateBookingData} />}
                        />
                    </Routes>
                </div>
            </div>
        </Router>
    );
}

export default App;