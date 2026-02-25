import { Component, OnInit, OnDestroy } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Chart } from 'chart.js';
import 'chart.js/auto';
import { OrderService } from 'src/app/core/services/order.service';
import { Order } from 'src/app/core/models/order.model';
import { environment } from 'src/environments';

@Component({
  selector: 'app-admin-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class AdminDashboardComponent implements OnInit, OnDestroy {

  totalOrders = 0;
  totalUsers = 0;
  deliveredOrders = 0;
  pendingOrders = 0;
  totalRevenue = 0;

  categoryCounts: Record<string, number> = {};
  legendData: { name: string; color: string }[] = [];

  categoryChart: Chart | null = null;
  monthlyChart: Chart | null = null;

 monthNames = [
  'Jan','Feb','Mar','Apr','May','Jun',
  'Jul','Aug','Sep','Oct','Nov','Dec'
];


  private api = environment.apiUrl;

  constructor(
    private orderService: OrderService,
    private http: HttpClient
  ) {}

  ngOnInit(): void {
    this.loadDashboard();
  }

  ngOnDestroy(): void {
    this.categoryChart?.destroy();
    this.monthlyChart?.destroy();
  }


  loadDashboard(): void {

    this.orderService.getAllOrders().subscribe(res => {

      const orders: Order[] = res.data;

      this.totalOrders = orders.length;
      this.deliveredOrders = orders.filter(o => o.status === 'Delivered').length;
      this.pendingOrders = orders.filter(o => o.status === 'Pending').length;

      this.totalRevenue = orders
        .filter(o => o.status === 'Delivered')
        .reduce((sum, o) => sum + o.total, 0);

      this.buildCategoryCounts(orders);

    
      setTimeout(() => {
        this.renderCategoryChart();
        this.renderMonthlyChart(this.calculateMonthlyRevenue(orders));
      }, 0);
    });

   
    this.http.get<any[]>(
      `${this.api}/User/GetAll`,
      { withCredentials: true }
    ).subscribe(users => {
      this.totalUsers = users.length;
    });
  }

  

  private buildCategoryCounts(orders: Order[]): void {
    this.categoryCounts = {};

    orders.forEach(order => {
      order.items.forEach(item => {
        if (!item.category) return;

        this.categoryCounts[item.category] =
          (this.categoryCounts[item.category] || 0) + item.quantity;
      });
    });

    this.legendData = Object.keys(this.categoryCounts).map(cat => ({
      name: cat,
      color: this.colorFromText(cat)
    }));
  }

 

  calculateMonthlyRevenue(orders: Order[]): number[] {
    const year = new Date().getFullYear();
    const revenue = new Array(12).fill(0);

    orders.forEach(o => {
      if (o.status !== 'Delivered') return;

      const d = new Date(o.date);
      if (d.getFullYear() !== year) return;

      revenue[d.getMonth()] += o.total;
    });

    return revenue;
  }

  colorFromText(text: string): string {
    let hash = 0;
    for (let i = 0; i < text.length; i++) {
      hash = text.charCodeAt(i) + ((hash << 5) - hash);
    }
    return `hsl(${hash % 360}, 70%, 60%)`;
  }

 

  renderCategoryChart(): void {
    this.categoryChart?.destroy();

    const ctx = document.getElementById('categoryChart') as HTMLCanvasElement;
    if (!ctx) return;

    this.categoryChart = new Chart(ctx, {
      type: 'doughnut',
      data: {
        labels: Object.keys(this.categoryCounts),
        datasets: [{
          data: Object.values(this.categoryCounts),
          backgroundColor: Object.keys(this.categoryCounts)
            .map(c => this.colorFromText(c))
        }]
      },
      options: {
        cutout: '70%',
        plugins: { legend: { display: false } }
      }
    }) as Chart; 
  }

  renderMonthlyChart(data: number[]): void {
  this.monthlyChart?.destroy();

  const ctx = document.getElementById('monthlyRevenueChart') as HTMLCanvasElement;
  if (!ctx) return;

  this.monthlyChart = new Chart(ctx, {
    type: 'line',
    data: {
      labels: this.monthNames,
      datasets: [{
        data,
        label: 'Revenue',
        fill: true,
        tension: 0.45,                
        borderWidth: 3,
        pointRadius: 4,               
        pointHoverRadius: 6,
        borderColor: '#6366F1',       
        backgroundColor: 'rgba(99,102,241,0.18)' 
      }]
    },
    options: {
      responsive: true,
      maintainAspectRatio: false,
      plugins: {
        legend: { display: false },
        tooltip: {
          callbacks: {
            label: (ctx) => ` ₹${ctx.parsed.y}`
          }
        }
      },
      scales: {
        x: {
          grid: { display: false }    
        },
        y: {
          beginAtZero: true,
          grid: {
            color: 'rgba(0,0,0,0.06)' 
          },
          ticks: {
            callback: (v: any) => `₹${v}`
          }
        }
      }
    }
  }) as Chart;
}

}
