// src/components/ProgressBar.jsx
import React from 'react';
import {useLocation, useNavigate} from 'react-router-dom';
import logo from '../assets/Logo.png';

const ProgressBar = () => {
    const location = useLocation();
    const navigate = useNavigate();

    const steps = [
        {path: '/postcode', label: 'Enter postcode'},
        {path: '/services', label: 'Choose services'},
        {path: '/time-slots', label: 'Select Date and Time'},
        {path: '/payment', label: 'Checkout'}
    ];

    const currentStepIndex = steps.findIndex(step => step.path === location.pathname);
    const progressPercentage = currentStepIndex >= 0 ? (currentStepIndex / (steps.length - 1)) * 100 : 0;

    const handleLogoClick = () => {
        window.location.href = 'https://www.mvmcleaning.com';
    };

    return (
        <div className="w-full">
            {/* Header */}
            <header
                className="w-full py-4 px-4 sm:px-6 grid grid-cols-3 items-center shadow-md"
                style={{backgroundColor: '#194376'}}
            >
                {/* Left: Brand Name */}
                <div
                    className="cursor-pointer hover:opacity-90 transition-opacity"
                    onClick={handleLogoClick}
                >
                    <h1 className="text-white font-bold text-3xl tracking-wide">
                        MvM<span className="text-[#46C6CE]">Cleaning</span>
                    </h1>
                </div>
                
                <div></div>

                {/* Right: Step Counter */}
                <div className="hidden md:flex items-center justify-end gap-3">
        <span className="text-white/90 text-sm font-medium">
            Step {currentStepIndex + 1} of {steps.length}
        </span>
                    <div className="flex items-center gap-1">
                        {steps.map((_, index) => (
                            <div
                                key={index}
                                className={`h-2 rounded-full transition-all duration-300 ${
                                    index <= currentStepIndex
                                        ? 'w-8 bg-[#46C6CE]'
                                        : 'w-6 bg-white/30'
                                }`}
                            />
                        ))}
                    </div>
                </div>
            </header>


            {/* Main Progress Bar Section */}
            <div className="w-full bg-gradient-to-b from-gray-50 to-white py-5 sm:py-6">
                <div className="max-w-5xl mx-auto px-4 sm:px-6">
                    <div className="relative">
                        {/* Background Track */}
                        <div className="absolute top-6 left-0 right-0 h-2 bg-gray-200 rounded-full shadow-inner"></div>

                        {/* Animated Progress Fill */}
                        <div
                            className="absolute top-6 left-0 h-2 rounded-full transition-all duration-700 ease-out shadow-sm"
                            style={{
                                background: 'linear-gradient(90deg, #194376 0%, #46C6CE 100%)',
                                width: `${progressPercentage}%`
                            }}
                        >
                            {/* Shimmer Effect */}
                            <div
                                className="absolute inset-0 bg-gradient-to-r from-transparent via-white/30 to-transparent animate-pulse"></div>
                        </div>

                        {/* Steps Container */}
                        <div className="relative flex justify-between">
                            {steps.map((step, index) => {
                                const isActive = index === currentStepIndex;
                                const isCompleted = index < currentStepIndex;
                                const isClickable = index <= currentStepIndex;

                                return (
                                    <div
                                        key={step.path}
                                        className="flex flex-col items-center relative z-10 flex-1"
                                    >
                                        {/* Step Circle */}
                                        <div
                                            className={`
                                                w-14 h-14 rounded-full flex items-center justify-center mb-3
                                                transition-all duration-500 transform
                                                ${isClickable ? 'cursor-pointer' : 'cursor-default'}
                                                ${isCompleted ? 'bg-[#194376] shadow-lg hover:shadow-xl' : ''}
                                                ${isActive ? 'bg-white shadow-2xl scale-110' : ''}
                                                ${!isActive && !isCompleted ? 'bg-white border-4 border-gray-300 shadow-md' : ''}
                                                ${isClickable && !isActive ? 'hover:scale-105 hover:shadow-lg' : ''}
                                            `}
                                            style={isActive ? {
                                                boxShadow: '0 0 0 4px rgba(70, 198, 206, 0.3), 0 10px 25px rgba(0, 0, 0, 0.15)'
                                            } : {}}
                                            onClick={() => isClickable && navigate(step.path)}
                                        >
                                            {/* Inner Circle for Active State */}
                                            {isActive && (
                                                <div
                                                    className="absolute inset-0 rounded-full animate-pulse"
                                                    style={{
                                                        background: 'linear-gradient(135deg, #194376, #46C6CE)',
                                                        opacity: 0.1
                                                    }}
                                                ></div>
                                            )}

                                            {isCompleted ? (
                                                <svg
                                                    className="w-7 h-7 text-white"
                                                    fill="currentColor"
                                                    viewBox="0 0 20 20"
                                                >
                                                    <path
                                                        fillRule="evenodd"
                                                        d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z"
                                                        clipRule="evenodd"
                                                    />
                                                </svg>
                                            ) : (
                                                <span
                                                    className={`
                                                        font-bold text-xl relative z-10
                                                        ${isActive ? 'text-[#194376]' : 'text-gray-500'}
                                                    `}
                                                >
                                                    {index + 1}
                                                </span>
                                            )}
                                        </div>

                                        {/* Step Label */}
                                        <div className="text-center px-2">
                                            <span className={`
                                                text-sm font-medium transition-all duration-300 block
                                                ${isActive ? 'text-[#194376] font-bold text-base' : ''}
                                                ${isCompleted ? 'text-[#194376] font-semibold' : ''}
                                                ${!isActive && !isCompleted ? 'text-gray-500' : ''}
                                            `}>
                                                {step.label}
                                            </span>

                                            {/* Active Indicator Dot */}
                                            {isActive && (
                                                <div className="mt-2 flex justify-center">
                                                    <div
                                                        className="w-2 h-2 rounded-full animate-pulse shadow-lg"
                                                        style={{backgroundColor: '#194376'}}
                                                    ></div>
                                                </div>
                                            )}
                                        </div>
                                    </div>
                                );
                            })}
                        </div>
                    </div>
                </div>
            </div>

        </div>
    );
};

export default ProgressBar;