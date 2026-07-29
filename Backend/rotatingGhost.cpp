#include <cmath>
#include <cstdio>
#include <cstring>
#include <unistd.h>

const int W = 120, H = 40;
char  buf[W * H];
int   col[W * H];
float zbuf[W * H];

// ANSI Farben
#define WHITE 97
#define BLACK 30
#define PINK 95
#define CYAN 96

float rotY = 0.0f;
float t = 0.0f;

const float K1 = 45.0f;
const float DIST = 90.0f;

// ================= Rotation =================
void rotateY(float &x, float &z, float a) {
    float nx =  x * cos(a) + z * sin(a);
    float nz = -x * sin(a) + z * cos(a);
    x = nx; z = nz;
}

// ================= Plot 3D =================
void plot3D(float x, float y, float z, char c, int color) {
    rotateY(x, z, rotY);

    float ooz = 1.0f / (z + DIST);
    int xp = (int)(W / 2 + K1 * ooz * x);
    int yp = (int)(H / 2 - K1 * ooz * y);

    if (xp < 0 || xp >= W || yp < 0 || yp >= H) return;
    int i = xp + yp * W;

    if (ooz > zbuf[i]) {
        zbuf[i] = ooz;
        buf[i] = c;
        col[i] = color;
    }
}

// ================= 2D Geist-Emoji-Maske =================
bool ghostMask(float x, float y) {
    // Kopf
    if (y > 6 && (x*x + (y-18)*(y-18)) < 18*18) return true;

    // Körper
    if (y <= 6 && y >= -16 && fabs(x) < 18) return true;

    // Wellen
    if (y < -16 && y > -20 && ((int)(x + 1000) % 6) < 3) return true;

    return false;
}

// ================= Gesicht (Front oder Back) =================
void drawFace(float zPos, bool mirrored, float floatY) {
    float xMul = mirrored ? -1.0f : 1.0f;

    // Augen
    for (float y = 10; y <= 14; y += 0.6f)
        for (float x = -2; x <= 2; x += 0.6f) {
            plot3D(xMul * (-7 + x), y + floatY, zPos, '@', BLACK);
            plot3D(xMul * ( 7 + x), y + floatY, zPos, '@', BLACK);
        }

    // Mund
    for (float x = -2.5f; x <= 2.5f; x += 0.6f)
        plot3D(xMul * x, 6 + floatY, zPos, 'o', PINK);
}

// ================= Geist =================
void drawGhost() {
    float floatY = sin(t) * 2.0f;

    // ===== Körper (Extrusion) =====
    for (float z = -6; z <= 6; z += 1.2f) {
        int bodyColor = (z > 0) ? WHITE : CYAN;

        for (float y = -22; y <= 30; y += 0.8f) {
            for (float x = -20; x <= 20; x += 0.8f) {
                if (ghostMask(x, y))
                    plot3D(x, y + floatY, z, '#', bodyColor);
            }
        }
    }

    // ===== ZWEI VORDERSEITEN =====
    drawFace( 7.5f, false, floatY); // vorne
    drawFace(-7.5f, true,  floatY); // hinten (gespiegelt!)
}

// ================= Main =================
void runAnimation() {
    //printf("\x1b[2J");
    system("cls");

    memset(buf,  ' ', sizeof(buf));
    memset(col,   0,  sizeof(col));
    memset(zbuf,  0,  sizeof(zbuf));

    rotY += 0.04f;
    t    += 0.07f;

    drawGhost();

    printf("\x1b[H");
    for (int i = 0; i < W * H; i++) {
        if (col[i])
            printf("\x1b[%dm%c", col[i], buf[i]);
        else
            putchar(' ');

        if (i % W == W - 1)
            printf("\x1b[0m\n");
    }

    printf("(Press Enter to Resume!)");

    usleep(12000);
}

void animateGhost() {
    runAnimation();
}
