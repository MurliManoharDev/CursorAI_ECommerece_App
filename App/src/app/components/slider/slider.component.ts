import { Component, OnInit, OnDestroy } from '@angular/core';

interface Slide {
  imageUrl: string;
  title: string;
  subtitle?: string;
  buttonText: string;
  features?: string[];
}

@Component({
  selector: 'app-slider',
  templateUrl: './slider.component.html',
  styleUrls: ['./slider.component.scss']
})
export class SliderComponent implements OnInit, OnDestroy {

  slides: Slide[] = [
    {
      imageUrl: 'assets/images/slider1.png',
      title: 'aPodOs\nWork wonders\nwith easy',
      subtitle: 'Experience aPod 2023 with new\ntechnology from $769',
      buttonText: 'discover now'
    },
    {
      imageUrl: 'assets/images/slider2.png',
      title: 'pc gaming\ncases',
      subtitle: 'SAle up to\n50% off',
      buttonText: 'buy now'
    },
    {
      imageUrl: 'assets/images/slider3.png',
      title: 'Noise Cancelling\nHeadphone',
      buttonText: 'buy now',
      features: [
        'Boso Over-Ear Headphone',
        'Wifi, Voice Assistant,',
        'Low latency game mde'
      ]
    }
  ];

  currentIndex = 0;
  private timeoutId: ReturnType<typeof setInterval> | null = null;

  ngOnInit(): void {
    this.startAutoSlide();
  }

  ngOnDestroy(): void {
    if (this.timeoutId) {
      clearInterval(this.timeoutId);
    }
  }

  prevSlide() {
    this.currentIndex = (this.currentIndex === 0) ? (this.slides.length - 1) : (this.currentIndex - 1);
  }

  nextSlide() {
    this.currentIndex = (this.currentIndex + 1) % this.slides.length;
  }

  private startAutoSlide() {
    this.timeoutId = setInterval(() => {
      this.nextSlide();
    }, 5000);
  }
} 