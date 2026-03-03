import React, { useState, useEffect, useMemo } from 'react';
import {
    BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, PieChart, Pie, Cell,
    ResponsiveContainer, Legend, LineChart, Line, RadarChart, Radar, PolarGrid,
    PolarAngleAxis, PolarRadiusAxis, Rectangle, ReferenceLine
} from 'recharts';
import api from '../api/api';
import { format, subMonths, startOfMonth, endOfMonth, parseISO } from 'date-fns';
import { ru } from 'date-fns/locale';
import { Download, Calendar, RefreshCw, Eye, TrendingUp } from 'lucide-react';
import type { Patient } from '../TypesFromServer/Patient';

const AnalyticsDashboard = () => {
    const [patientData, setPatientData] = useState<Patient[]>([]);
    const [procedureData, setProcedureData] = useState<any[]>([]);
    const [doctorData, setDoctorData] = useState<any[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');
    const [dateRange, setDateRange] = useState({
        from: subMonths(new Date(), 6),
        to: new Date()
    });
    const [selectedDoctor, setSelectedDoctor] = useState<string>('all');
    const [autoRefresh, setAutoRefresh] = useState(false);
    const [refreshInterval, setRefreshInterval] = useState<NodeJS.Timeout | null>(null);

    // Автообновление данных
    useEffect(() => {
        if (autoRefresh) {
            const interval = setInterval(() => {
                fetchData();
            }, 60000); // Обновление каждую минуту
            setRefreshInterval(interval);
        } else if (refreshInterval) {
            clearInterval(refreshInterval);
            setRefreshInterval(null);
        }

        return () => {
            if (refreshInterval) clearInterval(refreshInterval);
        };
    }, [autoRefresh]);

    // Получение данных из API
    const fetchData = async () => {
        try {
            setLoading(true);
            setError('');

            // Получаем данные о пациентах
            const patientsResponse = await api.get('/Patients/All');
            // Получаем данные о процедурах
            const proceduresResponse = await api.get('/Procedures/All');
            // Получаем данные о врачах
            const doctorsResponse = await api.get('/Doctors/All');

            // Обработка данных - извлекаем массивы из ответов
            const patients = Array.isArray(patientsResponse.data)
                ? patientsResponse.data
                : patientsResponse.data?.items || patientsResponse.data?.data || [];

            const procedures = Array.isArray(proceduresResponse.data)
                ? proceduresResponse.data
                : proceduresResponse.data?.items || proceduresResponse.data?.data || [];

            const doctors = Array.isArray(doctorsResponse.data)
                ? doctorsResponse.data
                : doctorsResponse.data?.items || doctorsResponse.data?.data || [];

            setPatientData(patients);
            setProcedureData(procedures);
            setDoctorData(doctors);

            console.log('Пациенты:', patients);
            console.log('Процедуры:', procedures);
            console.log('Врачи:', doctors);
        } catch (err) {
            setError('Ошибка при загрузке данных. Проверьте подключение к API и авторизацию.');
            console.error('Ошибка загрузки данных:', err);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchData();
    }, []);

    // Фильтрация данных по периоду и врачу
    const filteredProcedures = useMemo(() => {
        return procedureData.filter(proc => {
            // Фильтрация по дате
            let procedureDate;
            if (proc.scheduledDate) {
                procedureDate = new Date(proc.scheduledDate);
            } else if (proc.creationDate) {
                procedureDate = new Date(proc.creationDate);
            }

            if (!procedureDate || isNaN(procedureDate.getTime())) return false;

            const inDateRange = procedureDate >= dateRange.from && procedureDate <= dateRange.to;

            // Фильтрация по врачу
            const matchesDoctor = selectedDoctor === 'all' ||
                proc.doctor?.id === selectedDoctor ||
                proc.doctorId === selectedDoctor;

            return inDateRange && matchesDoctor;
        });
    }, [procedureData, dateRange, selectedDoctor]);

    // Фильтрация пациентов по периоду
    const filteredPatients = useMemo(() => {
        return patientData.filter(patient => {
            const creationDate = patient.creationDate ? new Date(patient.creationDate) : new Date();
            return creationDate >= dateRange.from && creationDate <= dateRange.to;
        });
    }, [patientData, dateRange]);

    // Анализ данных: распределение пациентов по возрастным группам
    const analyzeAgeGroups = () => {
        const ageGroups = {
            '0-18': 0,
            '19-30': 0,
            '31-45': 0,
            '46-60': 0,
            '60+': 0
        };

        filteredPatients.forEach(patient => {
            const age = patient.age || 0;
            if (age <= 18) ageGroups['0-18']++;
            else if (age <= 30) ageGroups['19-30']++;
            else if (age <= 45) ageGroups['31-45']++;
            else if (age <= 60) ageGroups['46-60']++;
            else ageGroups['60+']++;
        });

        return Object.entries(ageGroups).map(([name, value]) => ({ name, value }));
    };

    // Анализ данных: популярность процедур
    const analyzeProcedurePopularity = () => {
        const procedureMap = {};

        filteredProcedures.forEach(proc => {
            const typeName = proc.title || 'Неизвестно';
            procedureMap[typeName] = (procedureMap[typeName] || 0) + 1;
        });

        return Object.entries(procedureMap)
            .map(([name, value]) => ({ name, value, fill: getProcedureColor(name) }))
            .sort((a, b) => b.value - a.value)
            .slice(0, 8); // Топ-8 процедур
    };

    // Анализ данных: динамика процедур по месяцам
    // Заменить функцию calculateMonthRevenue
    const calculateMonthRevenue = (monthKey: string) => {
        return filteredProcedures.reduce((sum, proc) => {
            let procDate;
            if (proc.scheduledDate) {
                procDate = new Date(proc.scheduledDate);
            } else if (proc.creationDate) {
                procDate = new Date(proc.creationDate);
            }

            if (procDate && !isNaN(procDate.getTime())) {
                const procMonth = format(procDate, 'MMMM yyyy', { locale: ru });
                if (procMonth === monthKey) {
                    return sum + (proc.price || 0);
                }
            }
            return sum;
        }, 0);
    };

    // Заменить функцию analyzeMonthlyTrends
    const analyzeMonthlyTrends = () => {
        const monthCounts = {};
        const monthRevenue = {};
        const months = [
            'Январь', 'Февраль', 'Март', 'Апрель', 'Май', 'Июнь',
            'Июль', 'Август', 'Сентябрь', 'Октябрь', 'Ноябрь', 'Декабрь'
        ];

        filteredProcedures.forEach(proc => {
            let date;
            if (proc.scheduledDate) {
                date = new Date(proc.scheduledDate);
            } else if (proc.creationDate) {
                date = new Date(proc.creationDate);
            }

            if (!date || isNaN(date.getTime())) return;

            const monthIndex = date.getMonth();
            const year = date.getFullYear();
            const monthName = months[monthIndex];
            const key = `${monthName} ${year}`;

            monthCounts[key] = (monthCounts[key] || 0) + 1;
            monthRevenue[key] = (monthRevenue[key] || 0) + (proc.price || 0);
        });

        // Создаем массив для всех месяцев в диапазоне
        const result = [];
        let currentDate = new Date(dateRange.from);
        const endDate = new Date(dateRange.to);

        while (currentDate <= endDate) {
            const monthIndex = currentDate.getMonth();
            const year = currentDate.getFullYear();
            const monthName = months[monthIndex];
            const key = `${monthName} ${year}`;

            result.push({
                month: monthName,
                year: year,
                date: format(currentDate, 'MMM yyyy', { locale: ru }),
                count: monthCounts[key] || 0,
                revenue: monthRevenue[key] || 0
            });

            currentDate.setMonth(currentDate.getMonth() + 1);
        }

        return result;
    };

    // Анализ данных: загруженность врачей
    const analyzeDoctorWorkload = () => {
        const doctorStats = {};

        // Инициализируем статистику для всех врачей
        doctorData.forEach(doctor => {
            doctorStats[doctor.id] = {
                id: doctor.id,
                name: doctor.fullname || doctor.name || 'Неизвестный врач',
                procedures: 0,
                revenue: 0,
                avgPrice: 0
            };
        });

        // Считаем процедуры для каждого врача
        filteredProcedures.forEach(proc => {
            const doctorId = proc.doctorId || proc.doctor?.id;
            if (doctorId && doctorStats[doctorId]) {
                doctorStats[doctorId].procedures += 1;
                doctorStats[doctorId].revenue += proc.price || 0;
            }
        });

        // Вычисляем средний чек
        Object.values(doctorStats).forEach((stat: any) => {
            if (stat.procedures > 0) {
                stat.avgPrice = Math.round(stat.revenue / stat.procedures);
            }
        });

        return Object.values(doctorStats)
            .sort((a: any, b: any) => b.procedures - a.procedures)
            .slice(0, 5);
    };

    // Анализ данных: повторные пациенты
    const analyzePatientRetention = () => {
        const patientProcedures = {};

        filteredProcedures.forEach(proc => {
            const patientId = proc.patientCardId;
            if (patientId) {
                patientProcedures[patientId] = (patientProcedures[patientId] || 0) + 1;
            }
        });

        let newPatients = 0;
        let returningPatients = 0;

        Object.values(patientProcedures).forEach(count => {
            if (count === 1) newPatients++;
            else returningPatients++;
        });

        return [
            { name: 'Новые пациенты', value: newPatients },
            { name: 'Вернувшиеся пациенты', value: returningPatients }
        ];
    };

    // Анализ данных: загруженность по дням недели
    const analyzeWeeklyLoad = () => {
        const days = ['Пн', 'Вт', 'Ср', 'Чт', 'Пт', 'Сб', 'Вс'];
        const dayCounts = Array(7).fill(0);

        filteredProcedures.forEach(proc => {
            let date;
            if (proc.scheduledDate) {
                date = new Date(proc.scheduledDate);
            } else if (proc.creationDate) {
                date = new Date(proc.creationDate);
            }

            if (date && !isNaN(date.getTime())) {
                const dayIndex = date.getDay();
                // В JavaScript getDay() возвращает 0 для воскресенья, 1 для понедельника и т.д.
                const normalizedIndex = dayIndex === 0 ? 6 : dayIndex - 1;
                dayCounts[normalizedIndex]++;
            }
        });

        return days.map((day, index) => ({
            day,
            procedures: dayCounts[index],
            fill: `#${Math.floor(Math.random() * 16777215).toString(16)}`
        }));
    };

    // Вычисление средней стоимости процедуры
    const calculateAvgProcedureCost = () => {
        if (!Array.isArray(filteredProcedures) || filteredProcedures.length === 0) return 0;

        const total = filteredProcedures.reduce((sum, proc) => {
            const price = proc.price || 0;
            return sum + price;
        }, 0);

        return Math.round(total / filteredProcedures.length);
    };


    const calculateMonthlyProcedureAverage = () => {
        if (filteredProcedures.length === 0) return 0;

        // Определяем диапазон для анализа - последние 6 месяцев
        const endDate = new Date();
        const startDate = new Date();
        startDate.setMonth(startDate.getMonth() - 6);

        // Фильтруем процедуры за период анализа
        const periodProcedures = filteredProcedures.filter(proc => {
            let date;
            if (proc.scheduledDate) {
                date = new Date(proc.scheduledDate);
            } else if (proc.creationDate) {
                date = new Date(proc.creationDate);
            }
            return date && date >= startDate && date <= endDate;
        });

        if (periodProcedures.length === 0) return 0;

        // Группируем процедуры по месяцам
        const monthlyCounts = {};
        periodProcedures.forEach(proc => {
            let date;
            if (proc.scheduledDate) {
                date = new Date(proc.scheduledDate);
            } else if (proc.creationDate) {
                date = new Date(proc.creationDate);
            }

            if (date) {
                const monthKey = `${date.getFullYear()}-${date.getMonth() + 1}`;
                monthlyCounts[monthKey] = (monthlyCounts[monthKey] || 0) + 1;
            }
        });

        // Вычисляем среднее количество процедур в месяц
        const monthCount = Object.keys(monthlyCounts).length;
        const totalProcedures = Object.values(monthlyCounts).reduce((sum, count) => sum + count, 0);

        return monthCount > 0 ? totalProcedures / monthCount : 0;
    };

    const forecastNextMonthProcedures = () => {
        const averagePerMonth = calculateMonthlyProcedureAverage();

        // Применяем коэффициент роста (10%)
        const growthRate = 1.1;

        return Math.round(averagePerMonth * growthRate);
    };

    const forecastNextMonthRevenue = () => {
        const forecastedProcedures = forecastNextMonthProcedures();
        const avgProcedureCost = calculateAvgProcedureCost();

        return forecastedProcedures * avgProcedureCost;
    };


    // Экспорт данных в CSV\
    const exportToCSV = () => {
        const monthlyTrends = analyzeMonthlyTrends();
        const procedurePopularity = analyzeProcedurePopularity();
        const doctorWorkload = analyzeDoctorWorkload();

        let csvContent = '';

        // Добавляем BOM для правильной кодировки UTF-8 в Excel
        csvContent += '\uFEFF';

        // Заголовки и данные для месячной динамики
        csvContent += 'Динамика по месяцам\n';
        csvContent += 'Месяц;Количество процедур;Выручка\n';
        monthlyTrends.forEach(item => {
            csvContent += `${item.date};${item.count};${item.revenue}\n`;
        });

        csvContent += '\nПопулярность процедур\n';
        csvContent += 'Название процедуры;Количество\n';
        procedurePopularity.forEach(item => {
            csvContent += `"${item.name}";${item.value}\n`;
        });

        csvContent += '\nЗагруженность врачей\n';
        csvContent += 'Врач;Количество процедур;Выручка;Средний чек\n';
        doctorWorkload.forEach((item: any) => {
            csvContent += `"${item.name}";${item.procedures};${item.revenue};${item.avgPrice}\n`;
        });

        // Создаем blob с правильной кодировкой
        const blob = new Blob([csvContent], {
            type: 'text/csv;charset=utf-8;'
        });

        // Создаем ссылку для скачивания
        const url = URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.setAttribute('href', url);
        link.setAttribute('download', `аналитика_клиники_${format(new Date(), 'dd_MM_yyyy')}.csv`);
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
        URL.revokeObjectURL(url);
    };

    // Генерация цвета для типа процедуры
    const getProcedureColor = (procedureName: string) => {
        const colors = ['#0088FE', '#00C49F', '#FFBB28', '#FF8042', '#A445ED', '#8884d8', '#82ca9d', '#ffc658'];
        const hash = procedureName.split('').reduce((acc, char) => {
            return acc + char.charCodeAt(0);
        }, 0);
        return colors[hash % colors.length];
    };

    if (loading) {
        return (
            <div className="analytics-container">
                <h1 className="page-title">Аналитическая панель</h1>
                <div className="loading-spinner">
                    <div className="spinner"></div>
                    <p>Загрузка данных...</p>
                </div>
            </div>
        );
    }

    if (error) {
        return (
            <div className="analytics-container error">
                <h1 className="page-title">Аналитическая панель</h1>
                <div className="error-card">
                    <div className="error-icon">⚠️</div>
                    <p className="error-message">{error}</p>
                    <button onClick={fetchData} className="retry-button">
                        <RefreshCw size={16} /> Попробовать снова
                    </button>
                </div>
            </div>
        );
    }
    // Расчет процента роста
    const calculateGrowthPercentage = () => {
        if (filteredProcedures.length === 0) return 0;

        // Берем данные за предыдущий период такой же длины
        const prevPeriodStart = new Date(dateRange.from);
        prevPeriodStart.setMonth(prevPeriodStart.getMonth() - (
            dateRange.to.getMonth() - dateRange.from.getMonth() +
            (dateRange.to.getFullYear() - dateRange.from.getFullYear()) * 12
        ));

        const prevPeriodEnd = new Date(dateRange.from);
        prevPeriodEnd.setDate(prevPeriodEnd.getDate() - 1);

        const prevProcedures = procedureData.filter(proc => {
            let procedureDate;
            if (proc.scheduledDate) {
                procedureDate = new Date(proc.scheduledDate);
            } else if (proc.creationDate) {
                procedureDate = new Date(proc.creationDate);
            }

            return procedureDate && procedureDate >= prevPeriodStart && procedureDate <= prevPeriodEnd;
        });

        if (prevProcedures.length === 0) return filteredProcedures.length > 0 ? 100 : 0;

        return Math.round(((filteredProcedures.length - prevProcedures.length) / prevProcedures.length) * 100);
    };

    // Общая статистика
    const growthPercent = calculateGrowthPercentage();
    const retentionRate = filteredProcedures.length > 0
        ? Math.round((analyzePatientRetention().find(p => p.name === 'Вернувшиеся пациенты')?.value || 0) / filteredProcedures.length * 100)
        : 0;

    const totalRevenue = filteredProcedures.reduce((sum, proc) => sum + (proc.price || 0), 0);
    const avgRevenuePerPatient = filteredPatients.length > 0
        ? Math.round(totalRevenue / filteredPatients.length)
        : 0;

    return (
        <div className="analytics-container">
            <div className="header-section">
                <h1 className="page-title">Аналитическая панель клиники</h1>
                <div className="header-actions">
                    <button
                        onClick={fetchData}
                        className="action-button refresh-button"
                        title="Обновить данные"
                    >
                        <RefreshCw size={18} />
                    </button>
                    <button
                        onClick={exportToCSV}
                        className="action-button export-button"
                        title="Экспортировать данные"
                    >
                        <Download size={18} /> Экспорт
                    </button>
                </div>
            </div>

            <div className="control-panel">
                <div className="date-range-selector">
                    <label><Calendar size={16} /> Период анализа:</label>
                    <input
                        type="date"
                        value={format(dateRange.from, 'yyyy-MM-dd')}
                        onChange={(e) => setDateRange(prev => ({ ...prev, from: new Date(e.target.value) }))}
                        className="date-input"
                    />
                    <span>—</span>
                    <input
                        type="date"
                        value={format(dateRange.to, 'yyyy-MM-dd')}
                        onChange={(e) => setDateRange(prev => ({ ...prev, to: new Date(e.target.value) }))}
                        className="date-input"
                    />
                </div>

                <div className="doctor-filter">
                    <label>Врач:</label>
                    <select
                        value={selectedDoctor}
                        onChange={(e) => setSelectedDoctor(e.target.value)}
                        className="doctor-select"
                    >
                        <option value="all">Все врачи</option>
                        {doctorData.map(doctor => (
                            <option key={doctor.id} value={doctor.id}>
                                {doctor.fullname || doctor.name}
                            </option>
                        ))}
                    </select>
                </div>

                <div className="auto-refresh">
                    <label>
                        <input
                            type="checkbox"
                            checked={autoRefresh}
                            onChange={(e) => setAutoRefresh(e.target.checked)}
                        />
                        Автообновление
                    </label>
                </div>
            </div>

            {/* Ключевые показатели */}
            <div className="kpi-cards">
                <div className="kpi-card primary">
                    <h3>Выручка</h3>
                    <p className="kpi-value">{totalRevenue.toLocaleString('ru-RU')} ₽</p>
                    <div className={`kpi-growth ${growthPercent >= 0 ? 'positive' : 'negative'}`}>
                        {growthPercent >= 0 ? '↑' : '↓'} {Math.abs(growthPercent)}% к пред. периоду
                    </div>
                </div>
                <div className="kpi-card">
                    <h3>Всего процедур</h3>
                    <p className="kpi-value">{filteredProcedures.length}</p>
                    <div className="kpi-subtext">Средний чек: {calculateAvgProcedureCost()} ₽</div>
                </div>
                <div className="kpi-card">
                    <h3>Всего пациентов</h3>
                    <p className="kpi-value">{filteredPatients.length}</p>
                    <div className="kpi-subtext">Удержание: {retentionRate}%</div>
                </div>
                <div className="kpi-card">
                    <h3>Доход на пациента</h3>
                    <p className="kpi-value">{avgRevenuePerPatient} ₽</p>
                    <div className="kpi-subtext">в среднем за период</div>
                </div>
            </div>

            {/* Графики */}
            <div className="charts-container">
                <div className="chart-section">
                    <h2><TrendingUp size={18} /> Динамика выручки</h2>
                    <ResponsiveContainer width="100%" height={300}>
                        <LineChart
                            data={analyzeMonthlyTrends()}
                            margin={{ top: 20, right: 30, left: 0, bottom: 5 }}
                        >
                            <CartesianGrid strokeDasharray="3 3" />
                            <XAxis dataKey="date" />
                            <YAxis tickFormatter={(value) => `${value.toLocaleString('ru-RU')} ₽`} />
                            <Tooltip
                                formatter={(value, name) => [
                                    name === 'revenue'
                                        ? `${Number(value).toLocaleString('ru-RU')} ₽`
                                        : value,
                                    name === 'revenue' ? 'Выручка' : 'Количество процедур'
                                ]}
                            />
                            <Legend />
                            <Line
                                type="monotone"
                                dataKey="revenue"
                                name="Выручка"
                                stroke="#8884d8"
                                strokeWidth={2}
                                dot={{ fill: '#8884d8' }}
                            />
                            <Line
                                type="monotone"
                                dataKey="count"
                                name="Количество процедур"
                                stroke="#82ca9d"
                                strokeWidth={2}
                                dot={{ fill: '#82ca9d' }}
                            />
                        </LineChart>
                    </ResponsiveContainer>
                </div>

                <div className="chart-section">
                    <h2><Eye size={18} /> Распределение по возрастным группам</h2>
                    <ResponsiveContainer width="100%" height={300}>
                        <BarChart data={analyzeAgeGroups()} margin={{ top: 20, right: 30, left: 20, bottom: 5 }}>
                            <CartesianGrid strokeDasharray="3 3" />
                            <XAxis dataKey="name" />
                            <YAxis />
                            <Tooltip />
                            <Bar dataKey="value" fill="#8884d8" name="Количество пациентов" />
                        </BarChart>
                    </ResponsiveContainer>
                </div>

                <div className="chart-section full-width">
                    <div className="chart-header">
                        <h2>Популярность процедур</h2>
                        <div className="chart-legend">
                            {analyzeProcedurePopularity().map((item, index) => (
                                <div key={index} className="legend-item">
                                    <span className="legend-color" style={{ backgroundColor: item.fill }}></span>
                                    <span>{item.name}</span>
                                </div>
                            ))}
                        </div>
                    </div>
                    <ResponsiveContainer width="100%" height={300}>
                        <PieChart>
                            <Pie
                                data={analyzeProcedurePopularity()}
                                cx="50%"
                                cy="50%"
                                innerRadius={80}
                                outerRadius={110}
                                paddingAngle={2}
                                dataKey="value"
                                nameKey="name"
                                labelLine={false}
                            >
                                {analyzeProcedurePopularity().map((entry, index) => (
                                    <Cell key={`cell-${index}`} fill={entry.fill} />
                                ))}
                            </Pie>
                            <Tooltip />
                        </PieChart>
                    </ResponsiveContainer>
                </div>

                <div className="chart-section">
                    <h2>Загруженность врачей</h2>
                    <ResponsiveContainer width="100%" height={300}>
                        <BarChart
                            data={analyzeDoctorWorkload()}
                            layout="vertical"
                            margin={{ top: 20, right: 30, left: 0, bottom: 5 }}
                        >
                            <CartesianGrid strokeDasharray="3 3" />
                            <XAxis type="number" />
                            <YAxis
                                dataKey="name"
                                type="category"
                                width={120}
                                tick={{ fontSize: 12 }}
                            />
                            <Tooltip
                                formatter={(value, name) => [
                                    name === 'revenue'
                                        ? `${Number(value).toLocaleString('ru-RU')} ₽`
                                        : value,
                                    name === 'revenue' ? 'Выручка' : 'Количество процедур'
                                ]}
                            />
                            <Bar dataKey="procedures" name="Процедуры" fill="#8884d8" barSize={20} />
                            <Bar dataKey="revenue" name="Выручка" fill="#82ca9d" barSize={20} />
                        </BarChart>
                    </ResponsiveContainer>
                </div>

                <div className="chart-section">
                    <h2>Повторные пациенты</h2>
                    <ResponsiveContainer width="100%" height={300}>
                        <PieChart>
                            <Pie
                                data={analyzePatientRetention()}
                                cx="50%"
                                cy="50%"
                                innerRadius={60}
                                outerRadius={90}
                                fill="#8884d8"
                                paddingAngle={5}
                                dataKey="value"
                                nameKey="name"
                                labelLine={false}
                            >
                                {analyzePatientRetention().map((entry, index) => (
                                    <Cell key={`cell-${index}`} fill={index === 0 ? '#0088FE' : '#00C49F'} />
                                ))}
                            </Pie>
                            <Tooltip />
                            <Legend />
                        </PieChart>
                    </ResponsiveContainer>
                </div>

                <div className="chart-section full-width">
                    <h2>Загруженность по дням недели</h2>
                    <ResponsiveContainer width="100%" height={300}>
                        <RadarChart data={analyzeWeeklyLoad()}>
                            <PolarGrid />
                            <PolarAngleAxis dataKey="day" />
                            <PolarRadiusAxis angle={30} domain={[0, Math.max(...analyzeWeeklyLoad().map(i => i.procedures)) + 5]} />
                            <Radar
                                name="Количество процедур"
                                dataKey="procedures"
                                stroke="#8884d8"
                                fill="#8884d8"
                                fillOpacity={0.6}
                            />
                            <Tooltip />
                            <Legend />
                        </RadarChart>
                    </ResponsiveContainer>
                </div>
            </div>

            {/* Прогноз на следующий месяц */}
            <div className="forecast-section">
                <h2>Прогноз на следующий месяц</h2>
                <div className="forecast-grid">
                    <div className="forecast-card">
                        <h3>Ожидаемое количество процедур</h3>
                        <p className="forecast-value">{forecastNextMonthProcedures()}</p>
                        <p className="forecast-note">на основе среднемесячных данных</p>
                    </div>
                    <div className="forecast-card">
                        <h3>Прогнозируемая выручка</h3>
                        <p className="forecast-value">{forecastNextMonthRevenue().toLocaleString('ru-RU')} ₽</p>
                        <p className="forecast-note">на следующий месяц</p>
                    </div>
                    <div className="forecast-card">
                        <h3>Рекомендации</h3>
                        <ul className="recommendations">
                            <li>Усилить маркетинг процедур: {analyzeProcedurePopularity()[0]?.name}</li>
                            <li>Рассмотреть расширение графика работы в {analyzeWeeklyLoad().reduce((a, b) => a.procedures > b.procedures ? a : b).day}</li>
                        </ul>
                    </div>
                </div>
                <p className="forecast-disclaimer">
                    Прогноз основан на среднем количестве процедур за последние 6 месяцев с учетом роста в 10%. Для более точного прогноза требуется анализ сезонности и маркетинговых активностей.
                </p>
            </div>
        </div>
    );
};

// Стили (можно вынести в CSS-файл)
const styles = `
  .analytics-container {
    font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
    max-width: 1600px;
    margin: 0 auto;
    padding: 20px;
    color: #333;
    background-color: #f8fafc;
  }
  
  .page-title {
    color: #1e293b;
    font-size: 2.2rem;
    margin-bottom: 20px;
    display: flex;
    align-items: center;
    justify-content: space-between;
  }
  
  .header-section {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 20px;
  }
  
  .header-actions {
    display: flex;
    gap: 10px;
  }
  
  .action-button {
    display: flex;
    align-items: center;
    gap: 6px;
    padding: 8px 16px;
    border-radius: 6px;
    border: none;
    font-weight: 500;
    cursor: pointer;
    transition: all 0.2s;
  }
  
  .refresh-button {
    background-color: #e2e8f0;
    color: #4a5568;
  }
  
  .export-button {
    background-color: #3b82f6;
    color: white;
  }
  
  .action-button:hover {
    transform: translateY(-2px);
    box-shadow: 0 4px 6px rgba(0,0,0,0.1);
  }
  
  .refresh-button:hover {
    background-color: #cbd5e1;
  }
  
  .export-button:hover {
    background-color: #2563eb;
  }
  
  .control-panel {
    display: flex;
    flex-wrap: wrap;
    gap: 20px;
    background: white;
    padding: 16px;
    border-radius: 10px;
    box-shadow: 0 2px 8px rgba(0,0,0,0.08);
    margin-bottom: 20px;
  }
  
  .date-range-selector, .doctor-filter, .auto-refresh {
    display: flex;
    align-items: center;
    gap: 10px;
  }
  
  .date-input {
    padding: 8px 12px;
    border: 1px solid #cbd5e1;
    border-radius: 6px;
    font-size: 14px;
  }
  
  .doctor-select {
    padding: 8px 12px;
    border: 1px solid #cbd5e1;
    border-radius: 6px;
    font-size: 14px;
    background-color: white;
  }
  
  .kpi-cards {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(240px, 1fr));
    gap: 20px;
    margin: 20px 0;
  }
  
  .kpi-card {
    background: white;
    border-radius: 12px;
    padding: 20px;
    text-align: center;
    box-shadow: 0 4px 6px rgba(0,0,0,0.05);
    transition: all 0.3s ease;
    border: 1px solid #e2e8f0;
  }
  
  .kpi-card:hover {
    transform: translateY(-5px);
    box-shadow: 0 10px 15px rgba(0,0,0,0.1);
  }
  
  .kpi-card.primary {
    background: linear-gradient(135deg, #3b82f6, #6366f1);
    color: white;
    border: none;
  }
  
  .kpi-card.primary h3 {
    color: rgba(255,255,255,0.9);
  }
  
  .kpi-card.primary .kpi-value {
    color: white;
  }
  
  h3 {
    font-size: 1.1rem;
    color: #4a5568;
    margin-bottom: 8px;
    font-weight: 600;
  }
  
  .kpi-value {
    font-size: 2.2rem;
    font-weight: bold;
    margin: 10px 0;
    color: #1e3a8a;
  }
  
  .kpi-card.primary .kpi-value {
    color: white;
  }
  
  .kpi-growth {
    font-size: 0.9rem;
    font-weight: 500;
    margin-top: 5px;
  }
  
  .kpi-growth.positive {
    color: #10b981;
  }
  
  .kpi-growth.negative {
    color: #ef4444;
  }
  
  .kpi-subtext {
    font-size: 0.85rem;
    color: #64748b;
    margin-top: 4px;
  }
  
  .charts-container {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(550px, 1fr));
    gap: 25px;
  }
  
  .chart-section.full-width {
    grid-column: 1 / -1;
  }
  
  .chart-section {
    background: white;
    border-radius: 12px;
    padding: 20px;
    box-shadow: 0 4px 6px rgba(0,0,0,0.05);
    border: 1px solid #e2e8f0;
  }
  
  h2 {
    color: #1e293b;
    font-size: 1.4rem;
    margin-bottom: 16px;
    display: flex;
    align-items: center;
    gap: 8px;
  }
  
  .error-card {
    background: #fef2f2;
    border: 1px solid #fecaca;
    border-radius: 8px;
    padding: 24px;
    text-align: center;
    margin: 40px auto;
    max-width: 600px;
  }
  
  .error-icon {
    font-size: 3rem;
    margin-bottom: 16px;
    color: #ef4444;
  }
  
  .error-message {
    color: #b91c1c;
    font-size: 1.1rem;
    margin-bottom: 20px;
  }
  
  .retry-button {
    background: #ef4444;
    color: white;
    border: none;
    padding: 10px 24px;
    border-radius: 6px;
    font-size: 1rem;
    cursor: pointer;
    display: inline-flex;
    align-items: center;
    gap: 8px;
    transition: background 0.2s;
  }
  
  .retry-button:hover {
    background: #dc2626;
  }
  
  .forecast-section {
    background: linear-gradient(135deg, #dbeafe, #bfdbfe);
    border-radius: 12px;
    padding: 25px;
    margin-top: 30px;
    border: 1px solid #93c5fd;
  }
  
  .forecast-value {
    font-size: 2rem;
    font-weight: bold;
    color: #1e40af;
    margin: 8px 0;
  }
  
  .forecast-note {
    color: #3b82f6;
    font-style: italic;
    font-size: 0.9rem;
  }
  
  .forecast-grid {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
    gap: 20px;
    margin: 20px 0;
  }
  
  .forecast-card {
    background: white;
    padding: 20px;
    border-radius: 8px;
    box-shadow: 0 2px 4px rgba(0,0,0,0.05);
  }
  
  .recommendations {
    text-align: left;
    margin-top: 10px;
    padding-left: 20px;
    color: #1e40af;
  }
  
  .recommendations li {
    margin-bottom: 8px;
  }
  
  .forecast-disclaimer {
    font-style: italic;
    color: #3b82f6;
    margin-top: 20px;
    font-size: 0.9rem;
  }
  
  .loading-spinner {
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    padding: 60px;
  }
  
  .spinner {
    width: 50px;
    height: 50px;
    border: 5px solid #e2e8f0;
    border-top: 5px solid #3b82f6;
    border-radius: 50%;
    animation: spin 1s linear infinite;
    margin-bottom: 20px;
  }
  
  @keyframes spin {
    0% { transform: rotate(0deg); }
    100% { transform: rotate(360deg); }
  }
  
  .chart-header {
    display: flex;
    justify-content: space-between;
    align-items: flex-start;
    margin-bottom: 15px;
  }
  
  .chart-legend {
    display: flex;
    flex-wrap: wrap;
    gap: 10px;
    max-height: 80px;
    overflow-y: auto;
  }
  
  .legend-item {
    display: flex;
    align-items: center;
    gap: 4px;
    font-size: 0.8rem;
  }
  
  .legend-color {
    display: inline-block;
    width: 12px;
    height: 12px;
    border-radius: 2px;
  }
  .doctor-select {
    padding: 8px 12px;
    border: 1px solid #cbd5e1;
    border-radius: 6px;
    font-size: 14px;
    background-color: white;
    color: #333; /* Добавлено: черный цвет текста */
}
`;

// Добавляем стили в head
if (!document.getElementById('analytics-styles')) {
    const styleSheet = document.createElement("style");
    styleSheet.id = 'analytics-styles';
    styleSheet.innerText = styles;
    document.head.appendChild(styleSheet);
}

export default AnalyticsDashboard;