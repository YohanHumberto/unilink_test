<template>
    <Pie 
    :options="chartOptions" 
    :data="chartData" 
    id="pie-chart" 
    dataset-id-key="label" 
    width="400"
        height="400" />
</template>
  
<script>
import { Pie } from 'vue-chartjs'
import { Chart as ChartJS, Title, Tooltip, Legend, BarElement, ArcElement, CategoryScale, LinearScale } from 'chart.js'
import ChartDataLabels from 'chartjs-plugin-datalabels';
ChartJS.register(Title, Tooltip, Legend, BarElement, ArcElement, CategoryScale, LinearScale)
ChartJS.register(ChartDataLabels);

import Elements from '../assets/data.js';

export default {
    name: 'RuletaComponent',
    components: { Pie },
    data() {
        return {
            chartData: {
                labels: Elements,
                datasets: [
                    {
                        backgroundColor: Elements.map(a => a.color),
                        data: Elements.map(a => a.value),
                    }
                ]
            },
            chartOptions: {
                responsive: false   ,
                maintainAspectRatio: false,
                plugins: {
                    tooltip: false,
                    legend: {
                        display: false,
                    },
                    datalabels: {
                        anchor: "end",
                        align: "start",
                        color: "#ffffff",
                        formatter: (_, context) => context.chart.data.labels[context.dataIndex].label,
                        font: { size: 18 },
                    }
                }
            }
        }
    },
}
</script>
