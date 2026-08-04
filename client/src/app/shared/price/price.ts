import { Component, Input } from '@angular/core';

/**
 * Rial prices carry 3 decimals, so every price in the shop is set the way a
 * price tag is: rials large, baisa small, currency smaller still.
 */
@Component({
  selector: 'app-price',
  template: `
    <span class="price" [class]="'price--' + size">
      <span class="price__rials">{{ rials }}</span><span class="price__baisa">.{{ baisa }}</span>
      <span class="price__currency">OMR</span>
    </span>
  `,
  styleUrl: './price.css',
})
export class Price {
  @Input({ required: true }) value = 0;
  @Input() size: 'sm' | 'md' | 'lg' = 'md';

  private get baisaTotal(): number {
    return Math.round((this.value ?? 0) * 1000);
  }

  get rials(): string {
    return Math.floor(this.baisaTotal / 1000).toLocaleString('en-US');
  }

  get baisa(): string {
    return (this.baisaTotal % 1000).toString().padStart(3, '0');
  }
}
